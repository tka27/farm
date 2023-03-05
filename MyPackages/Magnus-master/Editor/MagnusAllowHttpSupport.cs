using UnityEditor;
using System.IO;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

#if UNITY_IOS
using UnityEditor.iOS.Xcode;
#endif

namespace MagnusSdk.Core.Editor
{
    public class MagnusAllowHttpSupport :  IPostprocessBuildWithReport
    {
        public int callbackOrder => 0;
    
        public void OnPostprocessBuild(BuildReport report)
        {
#if UNITY_IOS
            BuildTarget buildTarget = report.summary.platform;
            string pathToBuiltProject = report.summary.outputPath;
    
            if (buildTarget == BuildTarget.iOS)
            {
                // Get plist
                string plistPath = pathToBuiltProject + "/Info.plist";
                PlistDocument plist = new PlistDocument();
                plist.ReadFromString(File.ReadAllText(plistPath));
    
                PlistElementDict allowsDict = plist.root.CreateDict("NSAppTransportSecurity");
    
                allowsDict.SetBoolean("NSAllowsArbitraryLoads", true);
    
                PlistElementDict exceptionsDict = allowsDict.CreateDict("NSExceptionDomains");
    
                PlistElementDict domainDict = exceptionsDict.CreateDict("ip-api.com");
                domainDict.SetBoolean("NSExceptionAllowsInsecureHTTPLoads", true);
                domainDict.SetBoolean("NSIncludesSubdomains", true);
    
                // Write to file
                File.WriteAllText(plistPath, plist.WriteToString());
            }
#endif
        }
    }   
}