using System.Threading;
using System.Windows.Forms;

using BadBoy.BackEnd;
using BadBoy.BackEnd.LowLevel;

namespace BadBoy
{
    public class BadBoyCore
    {
        private Thread applicationThread;

        public void Start()
        {
            applicationThread = new Thread(ExecutionThread);
            applicationThread.Start();
        }

        public void Stop()
        {
            Application.Exit();
            applicationThread.Join();
        }

        private static void ExecutionThread()
        {
            KeyboardController.OnWordTyped += AnalyseCall;

            if (KeyboardController.HookKeyboard())
            {
                BadBoyLogger.LogInfo("Started BadBoy service");
            }
            else
            {
                BadBoyLogger.LogError("BadBoy service failed to start");
                
                return;
            }

            Application.Run();
            
            if (KeyboardController.ReleaseKeyboard())
                BadBoyLogger.LogInfo("Stopped BadBoy service");
            else
                BadBoyLogger.LogError("BadBoy service failed to stop");
        }

        private static void AnalyseCall(string word)
        {
            if (Filter.Analyse(word, out string analysedWord))
            {
                BadBoyLogger.LogInfo($"{word} -> {analysedWord}");

                SendKeys.Send("^{BACKSPACE}");
                SendKeys.Send(analysedWord);
            }
            else
            {
                BadBoyLogger.LogInfo($"{word} is clear");
            }
        }
    }
}