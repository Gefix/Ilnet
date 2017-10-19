using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Net;
//using MySql.Data;
//using MySql.Data.MySqlClient;
using System.Xml;
using System.Xml.Linq;
using Microsoft.CSharp;

using ILNET;
using ILNET.Common;

// intermediate keywords: exit, queue, slave

namespace ILNET.Runtime
{
    public delegate object iliClassProxy(FunctionExit exit, FunctionQueue queue, EventQueue eventqueue);

    /*class*/
    class /*cname*/c_c1/*/cname*/ : iliClass
    {
        public static Hashtable g = new Hashtable();

        private static Dictionary<string, string> _nodeNames;
        private static Dictionary<string, Dictionary<string, bool>> _incomingTransitions;
        private static Dictionary<string, Dictionary<string, bool>> _outgoingTransitions;

        public override Dictionary<string, string> NodeNames
        {
            get
            {
                return _nodeNames;
            }
            set
            {
                _nodeNames = value;
            }
        }

        public override Dictionary<string, Dictionary<string, bool>> IncomingTransitions
        {
            get
            {
                return _incomingTransitions;
            }
            set
            {
                _incomingTransitions = value;
            }
        }

        public override Dictionary<string, Dictionary<string, bool>> OutgoingTransitions
        {
            get
            {
                return _outgoingTransitions;
            }
            set
            {
                _outgoingTransitions = value;
            }
        }

        public static bool _isInitialised = false;

        public /*cname*/c_c1/*/cname*/(FunctionExit exit, FunctionQueue queue, EventQueue eventqueue)
            : base(exit, queue, eventqueue)
        {
            entry = /*centry*/fn_start/*/centry*/;

            /*varinit*/
            /*cvarscope*/
            g/*/cvarscope*/["/*cvarname*/x/*/cvarname*/"] = /*cvarvalue*/1/*/cvarvalue*/;
            /*/varinit*/

            lock (i)
            {
                if (!_isInitialised)
                {
                    _isInitialised = true;

                    NodeNames = new Dictionary<string, string>(); 
                    IncomingTransitions = new Dictionary<string, Dictionary<string, bool>>();
                    OutgoingTransitions = new Dictionary<string, Dictionary<string, bool>>();

                    /*nodename*/
                    NodeNames.Add("/*cnodecodename*/x/*/cnodecodename*/", "/*cnoderealname*/x/*/cnoderealname*/");
                    /*/nodename*/

                    /*transition*/
                    if (!OutgoingTransitions.ContainsKey("/*cnodestartname*/x/*/cnodestartname*/"))
                        OutgoingTransitions.Add("/*cnodestartname*/x/*/cnodestartname*/", new Dictionary<string, bool>());
                    OutgoingTransitions["/*cnodestartname*/x/*/cnodestartname*/"].Add("/*cnodeendname*/x/*/cnodeendname*/", true);

                    if (!IncomingTransitions.ContainsKey("/*cnodeendname*/x/*/cnodeendname*/"))
                        IncomingTransitions.Add("/*cnodeendname*/x/*/cnodeendname*/", new Dictionary<string, bool>());
                    IncomingTransitions["/*cnodeendname*/x/*/cnodeendname*/"].Add("/*cnodestartname*/x/*/cnodestartname*/", true);
                    /*/transition*/

                    /*gvarinit*/
                    /*cvarscope*/
                    g/*/cvarscope*/["/*cvarname*/x/*/cvarname*/"] = /*cvarvalue*/1/*/cvarvalue*/;
                    /*/gvarinit*/
                }
            }
        }

        public /*cname*/c_c1/*/cname*/ go_copy()
        {
            /*cname*/
            c_c1/*/cname*/ c = new /*cname*/c_c1/*/cname*/((FunctionExit)i["exit"], (FunctionQueue)i["queue"], (EventQueue)i["eventqueue"]);
            c.l = clone(l);
            c.i = i;
            c.q = q;
            return c;
        }

        override public Hashtable go_exit()
        {
            Hashtable result = new Hashtable();
            // put all output vars here
            // and all sub-class nodes' results

            /*cresult*/

            /*cvarresult*/
            result["/*cvarname*/x/*/cvarname*/"] = /*cvarscope*/g/*/cvarscope*/["/*cvarname*/x/*/cvarname*/"];
            /*/cvarresult*/

            // all sub-class nodes
            /*cnoderesult*/
            if (l.Contains("/*cnodename*/finish/*/cnodename*/")) result["/*cnodename*/finish/*/cnodename*/"] = (Hashtable)l["/*cnodename*/finish/*/cnodename*/"];
            /*/cnoderesult*/

            /*/cresult*/

            clone_hash_deep(result);

            return result;
        }

        /*cnodefn*/
        /*cnodeisexternal*/
        private static iliClassProxy /*cnodefnproxy*/c_c1_proxy/*/cnodefnproxy*/ = null;
        /*/cnodeisexternal*/
        public void /*cnodefnname*/fn_start/*/cnodefnname*/()
        {
            l["node"] = (FunctionExit)/*cnodefnnamecb*/fn_start_cb/*/cnodefnnamecb*/;

            /*cnodeisfn*/
            l["break"] = false;

            // do something
            try
            {
                /*cnodefncode*/
                g["x"] = (int)g["x"] + 2;
                if ((int)g["x"] > 5) return;
                /*/cnodefncode*/
                // /*fname*//*cnodefnname*/;/*/cnodefnname*//*/fname*/
            }
            catch (Exception ex)
            {
                error(ex.Message);
                error(ex.StackTrace);
                error("- - - - -");

                return;
            }

            if ((bool)l["break"]) return;
            /*/cnodeisfn*/

            /*cnodeissubprocess*/
            /*csubname*/
            dynamic/*/csubname*/ c;
            /*cnodeisexternal*/
            if (/*cnodefnproxy*/c_c1_proxy/*/cnodefnproxy*/ == null)
            {
                Hashtable data = new Hashtable();
                data.Add("systemEvent", "systemLoad");
                data.Add("projectName", "/*csubprojectname*/path.to.project/*/csubprojectname*/");
                ((EventQueue)i["eventqueue"])(data, /*cnodefnnamecb*/fn_start_cb/*/cnodefnnamecb*/);
                if (!data.ContainsKey("assembly")) return;
                Assembly asm = (Assembly)data["assembly"];
                /*cnodefnproxy*/
                c_c1_proxy/*/cnodefnproxy*/ = FastObjectFactory.CreateObjectFactory(asm.GetType("ILNET.Runtime.c_/*csubprocessname*/process/*/csubprocessname*/"));
            }

            c = /*cnodefnproxy*/c_c1_proxy/*/cnodefnproxy*/(/*cnodefnnamecb*/fn_start_cb/*/cnodefnnamecb*/, (FunctionQueue)i["queue"], (EventQueue)i["eventqueue"]);
            /*/cnodeisexternal*/
            /*cnodeisinternal*/
            c = new /*csubname*/c_c1/*/csubname*/(/*cnodefnnamecb*/fn_start_cb/*/cnodefnnamecb*/, (FunctionQueue)i["queue"], (EventQueue)i["eventqueue"]);
            /*/cnodeisinternal*/
            /*csubparam*/
            c./*cparamscope*/i/*/cparamscope*/["/*cparamname*/x/*/cparamname*/"] = /*cparamvalue*/1/*/cparamvalue*/;
            /*/csubparam*/
            c.run();
            /*/cnodeissubprocess*/

            /*cnodeisfn*/
            this./*cnodefnnamecb*/fn_start_cb/*/cnodefnnamecb*/(null);
            /*/cnodeisfn*/
        }

        // call-back function
        public void /*cnodefnnamecb*/fn_start_cb/*/cnodefnnamecb*/(Hashtable result)
        {
            object node = l["node"];
            l.Remove("node");

            if (result != null)
            {
                l["/*cnodename*/finish/*/cnodename*/"] = result;

                /*cnodefncodecb*/
                g["x"] = (int)g["x"] + 2;
                if ((int)g["x"] > 5) return;
                /*/cnodefncodecb*/
            }

            // load next nodes

            l["previousNode"] = node;

            /*cnodefngoto*/
            go_to(new FunctionToCall(go_copy()./*cnodefngotoname*/fn_start/*/cnodefngotoname*/));
            /*/cnodefngoto*/

            l.Remove("previousNode");

            /*cnodeisfinal*/
            this.go_up();
            /*/cnodeisfinal*/
        }
        /*/cnodefn*/
    }
    /*/class*/

    public class Init
    {
        public void run(FunctionExit callback, EventQueue eventqueue)
        {
            /*sclass*/
            c_c1/*/sclass*/ job = new /*sclass*/c_c1/*/sclass*/(callback, null, eventqueue);
            job.run();
        }
    }

    public static class FastObjectFactory
    {
        private static readonly Hashtable creatorCache = Hashtable.Synchronized(new Hashtable());
        private readonly static Type coType = typeof(iliClassProxy);

        /// <summary>
        /// Create a new instance of the specified type
        /// </summary>
        /// <returns></returns>
        public static iliClassProxy CreateObjectFactory(Type t)
        {
            iliClassProxy c = creatorCache[t] as iliClassProxy;
            if (c == null)
            {
                lock (creatorCache.SyncRoot)
                {
                    c = creatorCache[t] as iliClassProxy;
                    if (c != null)
                    {
                        return c;
                    }

                    Type[] types = new Type[] { typeof(FunctionExit), typeof(FunctionQueue), typeof(EventQueue) };

                    DynamicMethod dynMethod = new DynamicMethod("DM$OBJ_FACTORY_" + t.Name, typeof(object), types, t);
                    ILGenerator ilGen = dynMethod.GetILGenerator();

                    ilGen.Emit(OpCodes.Ldarg_0);
                    ilGen.Emit(OpCodes.Ldarg_1);
                    ilGen.Emit(OpCodes.Ldarg_2);
                    ilGen.Emit(OpCodes.Newobj, t.GetConstructor(types));
                    ilGen.Emit(OpCodes.Ret);
                    c = (iliClassProxy)dynMethod.CreateDelegate(coType);
                    creatorCache.Add(t, c);
                }
            }
            return c;
        }
    }
    
    /*skip*/
    class Program
    {
        static public void callback(Hashtable results)
        {
            // print out the results
            System.Console.WriteLine(results["x"]);
            System.Console.WriteLine(results.Contains("/*cnodename*/finish/*/cnodename*/"));
        }

        static void Main(string[] args)
        {
            // run start-up class
            /*sclass*/
            c_main/*/sclass*/ job = new /*sclass*/c_main/*/sclass*/(callback, null, null);
            job.run();
        }
    }
    /*/skip*/
}
