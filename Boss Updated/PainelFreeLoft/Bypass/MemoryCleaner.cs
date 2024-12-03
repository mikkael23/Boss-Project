﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;


public class MemoryCleaner
{
    const int PROCESS_QUERY_INFORMATION = 0x0400;
    const int PROCESS_WM_READ = 0x0010;
    const int PAGE_READWRITE = 0x04;
    const int MEM_COMMIT = 0x1000;

    [DllImport("kernel32.dll")]
    public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool ReadProcessMemory(int hProcess, IntPtr lpBaseAddress, [Out] byte[] lpBuffer, int dwSize, out int lpNumberOfBytesRead);

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool WriteProcessMemory(int hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, uint nSize, out int lpNumberOfBytesWritten);

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool CloseHandle(IntPtr hObject);

    [DllImport("kernel32.dll")]
    public static extern void GetSystemInfo(out SYSTEM_INFO lpSystemInfo);

    [DllImport("kernel32.dll")]
    public static extern int VirtualQueryEx(IntPtr hProcess, IntPtr lpAddress, out MEMORY_BASIC_INFORMATION lpBuffer, int dwLength);

    [StructLayout(LayoutKind.Sequential)]
    public struct MEMORY_BASIC_INFORMATION
    {
        public IntPtr BaseAddress;
        public IntPtr AllocationBase;
        public int AllocationProtect;
        public IntPtr RegionSize;
        public int State;
        public int Protect;
        public int Type;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SYSTEM_INFO
    {
        public ushort processorArchitecture;
        ushort reserved;
        public uint pageSize;
        public IntPtr minimumApplicationAddress;
        public IntPtr maximumApplicationAddress;
        public IntPtr activeProcessorMask;
        public uint numberOfProcessors;
        public uint processorType;
        public uint allocationGranularity;
        public ushort processorLevel;
        public ushort processorRevision;
    }

    private static async Task<Process> GetProcessByNameAsync(string processName)
    {
        return await Task.Run(() =>
        {
            Process[] processes = Process.GetProcessesByName(processName);
            return processes.Length > 0 ? processes[0] : null;
        });
    }

    static async Task<ServiceController> GetServiceByNameAsync(string serviceName)
    {
        return await Task.Run(() =>
        {
            ServiceController[] services = ServiceController.GetServices();
            foreach (ServiceController service in services)
            {
                if (service.ServiceName.StartsWith(serviceName, StringComparison.OrdinalIgnoreCase))
                {
                    return service;
                }
            }
            return null;
        });
    }

    static async Task<int> GetProcessIdFromServiceAsync(ServiceController service)
    {
        return await Task.Run(() =>
        {
            using (ManagementObject serviceObj = new ManagementObject($"Win32_Service.Name='{service.ServiceName}'"))
            {
                object processIdObj = serviceObj["ProcessId"];
                if (processIdObj != null && int.TryParse(processIdObj.ToString(), out int processId))
                {
                    return processId;
                }
            }
            return 0;
        });
    }

    public class CliArgs
    {
        public List<string> searchterm { get; set; }
        public int prepostfix { get; set; }
        public int delay { get; set; }
        public string mode { get; set; }
    }

    public static async Task<bool> IsGitHubLinkPublic(string link)
    {
        try
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(link);
                return response.StatusCode == HttpStatusCode.OK;
            }
        }
        catch (HttpRequestException)
        {
            return false;
        }
    }

    public static async Task Main(string[] args)
    {
        await ExecuteMemoryCleaningAsync();
        Environment.Exit(0);
    }
    public static async Task ExecuteMemoryCleaningAsync()
    {
        Dictionary<string, List<string>> processToSearchStrings = new Dictionary<string, List<string>>
        {
            { "DPS", new List<string> { "AnyDesk.exe", "AnyDesk", "!!AnyDesk!", "!!AnyDesk" } },
            { "lsass", new List<string> { "authguard", "*.authguard.net0", "AnyDesk.exe", "api.auth", "*.github.io", "raw.githubusercontent.com" } },
            { "Dnscache", new List<string> { "authguard", "*.authguard.net0", "AnyDesk.exe", "api.auth", "*.github.io", "raw.githubusercontent.com" } },
            { "diagtrack", new List<string> { "2091", "2092", "!!", } }};


        foreach (var kvp in processToSearchStrings)
        {
            string processName = kvp.Key;
            List<string> searchStrings = kvp.Value;

            Process process = await GetProcessByNameAsync(processName);
            if (process != null)
            {
                foreach (string searchString in searchStrings)
                {
                    CliArgs myargs = new CliArgs
                    {
                        searchterm = new List<string> { searchString },
                        prepostfix = 10,
                        delay = 1000,
                        mode = "stdio"
                    };

                    var targetStrings = await Task.Run(() => memScanString(process, myargs));

                    if (targetStrings.Count > 0)
                    {
                        await Task.Run(() => ReplaceStringInProcessMemory(process, targetStrings));
                    }
                }
            }
            else
            {
                ServiceController service = await GetServiceByNameAsync(processName);
                if (service != null)
                {
                    int processId = await GetProcessIdFromServiceAsync(service);

                    if (processId > 0)
                    {
                        Process associatedProcess = Process.GetProcessById(processId);

                        foreach (string searchString in searchStrings)
                        {
                            CliArgs myargs = new CliArgs
                            {
                                searchterm = new List<string> { searchString },
                                prepostfix = 10,
                                delay = 1000,
                                mode = "stdio"
                            };

                            var targetStrings = await Task.Run(() => memScanString(associatedProcess, myargs));

                            if (targetStrings.Count > 0)
                            {
                                await Task.Run(() => ReplaceStringInProcessMemory(associatedProcess, targetStrings));
                            }
                        }
                    }
                }
            }
        }
    }

    public static Dictionary<long, string> memScanString(Process process, CliArgs myargs)
    {
        IntPtr processHandle = OpenProcess(PROCESS_QUERY_INFORMATION | PROCESS_WM_READ, false, process.Id);

        SYSTEM_INFO sys_info = new SYSTEM_INFO();
        GetSystemInfo(out sys_info);

        IntPtr proc_min_address = sys_info.minimumApplicationAddress;
        IntPtr proc_max_address = sys_info.maximumApplicationAddress;

        var targetStrings = new Dictionary<long, string>();

        while (proc_min_address.ToInt64() < proc_max_address.ToInt64())
        {
            VirtualQueryEx(processHandle, proc_min_address, out MEMORY_BASIC_INFORMATION mem_basic_info, Marshal.SizeOf(typeof(MEMORY_BASIC_INFORMATION)));

            if (mem_basic_info.Protect == PAGE_READWRITE && mem_basic_info.State == MEM_COMMIT)
            {
                byte[] buffer = new byte[(int)mem_basic_info.RegionSize];

                ReadProcessMemory(processHandle.ToInt32(), mem_basic_info.BaseAddress, buffer, (int)mem_basic_info.RegionSize, out int bytesRead);

                string memString = Encoding.Default.GetString(buffer);

                foreach (string searchString in myargs.searchterm)
                {
                    List<byte[]> encodedSearchBuffers = EncodeBuffer(searchString);

                    foreach (var searchBuffer in encodedSearchBuffers)
                    {
                        int startIndex = 0;

                        while ((startIndex = IndexOf(buffer, searchBuffer, startIndex)) != -1)
                        {
                            IntPtr address = (IntPtr)((long)mem_basic_info.BaseAddress + startIndex);
                            int length = searchBuffer.Length;

                            long addressKey = address.ToInt64();
                            if (!targetStrings.ContainsKey(addressKey))
                            {
                                targetStrings.Add(addressKey, Encoding.Default.GetString(buffer, startIndex, length));
                            }

                            startIndex += searchBuffer.Length;
                        }
                    }
                }
            }

            long size = mem_basic_info.RegionSize.ToInt64();
            if (size > int.MaxValue)
            {
                size = int.MaxValue;
            }
            proc_min_address = IntPtr.Add(mem_basic_info.BaseAddress, (int)size);
        }

        CloseHandle(processHandle);

        return targetStrings;
    }

    public static int IndexOf(byte[] haystack, byte[] needle, int start = 0)
    {
        for (int i = start; i <= haystack.Length - needle.Length; i++)
        {
            bool match = true;
            for (int j = 0; j < needle.Length; j++)
            {
                if (haystack[i + j] != needle[j])
                {
                    match = false;
                    break;
                }
            }

            if (match) return i;
        }
        return -1;
    }

    public static List<byte[]> EncodeBuffer(string input)
    {
        var encodings = new List<Encoding> { Encoding.UTF8, Encoding.ASCII, Encoding.Unicode, Encoding.Default };
        var buffers = new List<byte[]>();

        foreach (var encoding in encodings)
        {
            buffers.Add(encoding.GetBytes(input));
        }

        return buffers;
    }

    public static void ReplaceStringInProcessMemory(Process process, Dictionary<long, string> targetStrings)
    {
        foreach (KeyValuePair<long, string> stringInMemory in targetStrings)
        {
            long address = stringInMemory.Key;
            string str = stringInMemory.Value;

            byte[] bytes = Encoding.Default.GetBytes(str);

            byte[] currentMemoryData = new byte[bytes.Length];
            if (ReadProcessMemory(process.Handle.ToInt32(), (IntPtr)address, currentMemoryData, currentMemoryData.Length, out int bytesRead))
            {
                if (Enumerable.SequenceEqual(bytes, currentMemoryData))
                {
                    byte[] replacementBytes = new byte[bytes.Length];

                    WriteProcessMemory(process.Handle.ToInt32(), (IntPtr)address, replacementBytes, (uint)replacementBytes.Length, out int num);
                }
            }
        }
    }
}
