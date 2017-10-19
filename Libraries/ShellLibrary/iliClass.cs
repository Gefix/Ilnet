using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Threading;
using System.IO;

namespace ILNET.Common
{
    public delegate void FunctionToCall();
    public delegate void FunctionQueue(FunctionToCall f);
    public delegate void FunctionExit(Hashtable result);
    public delegate int EventQueue(Hashtable data, FunctionExit f);

    public abstract class iliClass
    {
        public Hashtable l, i;

        public Queue q;

        public FunctionToCall entry;

        public iliClass()
        {
            l = new Hashtable();
            i = new Hashtable();
            i["slave"] = true;
        }

        public void init(FunctionExit exit, FunctionQueue queue, EventQueue eventqueue)
        {
            i["exit"] = exit;
            i["queue"] = queue;
            i["eventqueue"] = eventqueue;
        }

        public iliClass(FunctionExit exit, FunctionQueue queue, EventQueue eventqueue)
        {
            l = new Hashtable();
            i = new Hashtable();

            i["exit"] = exit;

            if (queue == null)
            {
                queue = go_to;
                q = new Queue();
                i["slave"] = false;
            }
            else
            {
                i["slave"] = true;
            }
            i["queue"] = queue;
            i["eventqueue"] = eventqueue;
        }

        virtual public void run_loop()
        {
            lock (i)
            {
                if (i.ContainsKey("running")) return;
                i["running"] = true;
            }
            for (; ; )
            {
                lock (q)
                {
                    while (q.Count > 0)
                    {
                        FunctionToCall f = (FunctionToCall)q.Dequeue();
                        f();
                        /*{
                            Thread thread = new Thread(delegate() { f(); });
                            thread.IsBackground = true;
                            thread.SetApartmentState(System.Threading.ApartmentState.STA);
                            thread.Start();
                        }*/
                    }
                    Monitor.Wait(q);
                }
            }
            lock (i)
            {
                i.Remove("running");
            }
        }

        virtual public void run()
        {
            if ((bool)i["slave"])
            {
                ((FunctionQueue)i["queue"])(entry);
                return;
            }

            if (entry != null) q.Enqueue(entry);
            run_loop();
        }

        // Enters a sleeping state until the event is fired
        virtual public void raise(string eventType, Hashtable data)
        {
            if (i["eventqueue"] != null)
            {
                data["systemEvent"] = eventType;
                ((EventQueue)i["eventqueue"])(clone(data), (FunctionExit)l["node"]);
            }

            l["break"] = true;
        }
        virtual public void raiseEvent(Hashtable data)
        {
            raise("raise", data);
        }

        virtual public void queryEvents(Hashtable data)
        {
            raise("query", data);
        }

        virtual public void fetchEvent(Hashtable data)
        {
            raise("fetch", data);
        }

        virtual public void answerEvent(Hashtable data)
        {
            raise("answer", data);
        }

        virtual public void closeEvent(Hashtable data)
        {
            raise("close", data);
        }

        virtual public void listenEvent(Hashtable data)
        {
            raise("listen", data);
        }

        virtual public void closeListener(Hashtable data)
        {
            raise("close", data);
        }

        public void clone_hash_deep(Hashtable table)
        {
            Object[] keys = new Object[table.Count];
            table.Keys.CopyTo(keys, 0);
            foreach (Object o in keys)
            {
                if (table[o] is ICloneable)
                {
                    table[o] = ((ICloneable)table[o]).Clone();
                }
                if (table[o] is Hashtable) clone_hash_deep((Hashtable)table[o]);
            }
        }

        public Hashtable clone(Hashtable table)
        {
            Hashtable result = (Hashtable)table.Clone();

            clone_hash_deep(result);
            return result;
        }

        virtual public void go_to(FunctionToCall f)
        {
            lock (i)
            {
                if ((bool)i["slave"])
                {
                    ((FunctionQueue)i["queue"])(f);
                    return;
                }
            }

            lock (q)
            {
                q.Enqueue(f);

                lock (q) Monitor.Pulse(q);
            }
        }

        abstract public Hashtable go_exit();

        virtual public void go_up()
        {
            FunctionExit f = (FunctionExit)i["exit"];
            f(go_exit());
        }

        abstract public Dictionary<string, string> NodeNames { get; set; }
        abstract public Dictionary<string, Dictionary<string, bool>> IncomingTransitions { get; set; }
        abstract public Dictionary<string, Dictionary<string, bool>> OutgoingTransitions { get; set; }

        public iliClass parent()
        {
            if (!i.ContainsKey("exit")) return null;

            return ((FunctionExit)i["exit"]).Target as iliClass;
        }

        public string caller()
        {
            if (!i.ContainsKey("exit")) return null;
            if (parent() == null) return null;

            return parent().NodeNames[((FunctionExit)i["exit"]).Method.Name];
        }

        public string previous()
        {
            if (!l.ContainsKey("previousNode")) return null;
            return NodeNames[((FunctionExit)l["previousNode"]).Method.Name];
        }

        public void log(object message)
        {
            StreamWriter w = new StreamWriter("C:\\Program Files\\ILNET\\ILNET Server\\Solution\\log.txt", true);
            w.WriteLine(message.ToString());
            w.Close();
        }

        public void error(object message)
        {
            StreamWriter w = new StreamWriter("C:\\Program Files\\ILNET\\ILNET Server\\Solution\\error.txt", true);
            w.WriteLine(message.ToString());
            w.Close();
        }
    }
}
