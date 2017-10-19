using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using Microsoft.CSharp;
using Evaluator;
using ILNET.Common;
using ILNET.Elements;

namespace ILNET.Compiler
{
    public class ProjectRunner
    {
        private Project m_project;
        private AssemblyResults m_asm;

        private Thread m_thread;

        FunctionExit m_callback;
        EventQueue m_eventqueue;

        int m_pid;
        int m_gid;

        public ProjectRunner(Project project, AssemblyResults asm)
        {
            m_project = project;
            m_asm = asm;
        }

        public Project Project { get { return m_project; } }

        public Assembly Assembly { get { return m_asm.Assembly; } }

        public ProjectRunner Copy()
        {
            return new ProjectRunner(m_project, m_asm);
        }

        private void RunThread()
        {
            Type tRun = m_asm.Assembly.GetType("ILNET.Runtime.Init");

            Object[] pars = new Object[2];

            pars[0] = (FunctionExit)RunnerCallback;
            pars[1] = (EventQueue)RunnerEventqueue;

            Object c = Activator.CreateInstance(tRun, false);

            tRun.GetMethod("run").Invoke(c, pars);
        }

        public void SetPID(int pid)
        {
            m_pid = pid;
        }

        public void SetGID(int gid)
        {
            m_gid = gid;
        }

        public void Run(FunctionExit callback, EventQueue eventqueue)
        {
            m_callback = callback;
            m_eventqueue = eventqueue;

            m_thread = new Thread(RunThread);
            m_thread.Start();
        }

        void RunnerCallback(Hashtable results)
        {
            results["pid"] = m_pid;
            results["gid"] = m_gid;
            m_callback(results);
        }

        int RunnerEventqueue(Hashtable data, FunctionExit listener)
        {
            data["pid"] = m_pid;
            data["gid"] = m_gid;
            return m_eventqueue(data, listener);
        }

        public void Kill()
        {
            if (m_thread != null)
            {
                m_thread.Abort();
                m_thread = null;
            }
        }

        public bool IsRunning()
        {
            if (m_thread == null) return false;
            
            return m_thread.IsAlive;
        }
    }

    public class ProjectCompiler
    {
        public ProjectRunner Compile(string source)
        {
            return Compile(Project.Deserialize(source));
        }
        public ProjectRunner Compile(Project project)
        {
            return new ProjectRunner(project, CompileAssembly(project.Compile()));
        }

        public static string FullReference(string relativeReference)
        {
            Assembly a = Assembly.GetExecutingAssembly();
            string path = Path.GetDirectoryName(a.Location);

            string fullReference = Path.Combine(path, relativeReference);
            if (File.Exists(fullReference))
                return fullReference;
            else
            {
                // Strip off any trailing ".dll" if present.
                if (string.Compare(relativeReference.Substring(relativeReference.Length - 4), ".dll", true) == 0) fullReference = relativeReference.Substring(0, relativeReference.Length - 4);
                else fullReference = relativeReference;


                try
                {
                    Assembly tempAssembly =
                    Assembly.LoadWithPartialName(fullReference);
                    return tempAssembly.Location;
                }
                catch
                {
                    return relativeReference;
                }
            }
        }

        static private AssemblyResults CompileAssembly(String code)
        {
            try
            {
                StreamWriter w = new StreamWriter("C:\\Program Files\\ILNET\\ILNET Server\\Solution\\project.cs");
                w.Write(code);
                w.Close();

                Dictionary<string, string> providerOptions = new Dictionary<string, string>();
                providerOptions.Add("CompilerVersion", "v4.0");
                CodeDomProvider codeProvider = new CSharpCodeProvider(providerOptions);

                ICodeCompiler icc = codeProvider.CreateCompiler();
                CompilerParameters cp = new CompilerParameters()
                {
                    GenerateInMemory = false,
                    IncludeDebugInformation = Debugger.IsAttached,
                    TreatWarningsAsErrors = false,
                    WarningLevel = 0,
                    GenerateExecutable = false,
                    CompilerOptions = "/optimize+"
                };

                cp.ReferencedAssemblies.Add("System.dll");
                cp.ReferencedAssemblies.Add("System.Core.dll");
                cp.ReferencedAssemblies.Add("Microsoft.CSharp.dll");
                cp.ReferencedAssemblies.Add("System.Data.dll");
                cp.ReferencedAssemblies.Add("System.Drawing.dll");
                cp.ReferencedAssemblies.Add("System.Windows.Forms.dll");
                cp.ReferencedAssemblies.Add(FullReference("System.Xml.dll"));
                cp.ReferencedAssemblies.Add(FullReference("System.Xml.Linq.dll"));
                //cp.ReferencedAssemblies.Add(FullReference("MySql.Data.dll"));
                cp.ReferencedAssemblies.Add(FullReference("Common.dll"));
                AssemblyResults asm = Eval.CreateAssembly(icc, code, cp, new CSharpLanguage());
                return asm;
            }
            catch (CompilationException ce)
            {
                foreach (CompilerError error in ce.Errors)
                {
                    if (error.IsWarning) continue;
                    throw new Exception(Utils.Extract(code, "fname", code.NthIndexOf("\n", error.Line)).Substring(3), ce);
                }
                throw new Exception("", ce);
            }
        }
    }
}
