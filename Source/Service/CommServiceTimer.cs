using System;
using System.Timers;
using SG.GeneralLib;

namespace CommService
{
    static class CommServiceTimer
    {
        public static void TimerFunction(object sender, ElapsedEventArgs e)
        {
            Timer timerFunc = (Timer)sender;
            timerFunc.Enabled = false;

            try
            {
                // TODO: Put whatever code is needed here!

                Global.WriteLogFile("[Timer]: Timer function trigger.");
            }
            catch (Exception ex)
            {
                string error = String.Format("[Timer]: Exception: {0}",
                    ex.Message);

                Global.WriteEventLog(error);
                Global.WriteLogFile(error);
            }

            timerFunc.Enabled = true;
        }
    }
}
