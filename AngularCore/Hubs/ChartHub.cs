using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AngularCore.Models.Chart;
using Microsoft.AspNetCore.SignalR;

namespace AngularCore.Hubs
{
    /*
     Well, a Hub is a high-level pipeline that allows communication between client and server to call each others methods directly.
     So basically, a Hub is a communication foundation between client and server while using SignalR.
     */
    public class ChartHub : Hub
    {
    }

    public class TimerManager
    {
        private Timer timer;
        private AutoResetEvent autoResetEvent;
        private Action action;

        public DateTime TimerStarted { get; }

        public TimerManager(Action action)
        {
            this.action = action;
            autoResetEvent = new AutoResetEvent(false);
            timer = new Timer(Execute, autoResetEvent, 1000, 2000);
            TimerStarted = DateTime.Now;
        }

        public void Execute(object stateInfo)
        {
            action();

            if ((DateTime.Now - TimerStarted).Seconds > 60)
                timer.Dispose();
        }
    }

    public static class DataManager
    {
        public static List<ChartVM> GetData()
        {
            Random r = new Random();
            return new List<ChartVM>() {
                new ChartVM { Data = new List<int> { r.Next(1, 40) }, Label = "Data1" },
                new ChartVM { Data = new List<int> { r.Next(1, 40) }, Label = "Data2" },
                new ChartVM { Data = new List<int> { r.Next(1, 40) }, Label = "Data3" },
                new ChartVM { Data = new List<int> { r.Next(1, 40) }, Label = "Data4" }
            };
        }
    }
}
