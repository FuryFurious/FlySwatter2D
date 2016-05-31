using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace IGF_Semaine_Wrapper
{
    public class MyEventReceiver
    {
        private bool keyCombinationIsOpen;
        private int roundCount;

        private KeyboardListener kListener;
        private MainWindow mainWindow;


        public MyEventReceiver(KeyboardListener kListener, MainWindow mainWindow)
        {
            this.kListener = kListener;
            this.mainWindow = mainWindow;

            kListener.KeyDown += new RawKeyEventHandler(OnKeyDown);
        }

        void Reset()
        {
            roundCount = 0;
        }

       
        private void OnKeyDown(object sender, RawKeyEventArgs args)
        {
            //left control starts a combination sent by the game:
            if (args.Key == Key.LeftCtrl)
            {
                keyCombinationIsOpen = true;
            }

            else
            {
                //left control was the last key -> now check for the next key to invoke a semaine message
                if (keyCombinationIsOpen)
                {
                    //left control + F1 means that the round started:
                    if (args.Key == Key.F1)
                    {
                        mainWindow.SendSemaineMessage(string.Format("Round {0} started", roundCount));
                    }

                   //left control + F2 means that the round ended:
                    else if (args.Key == Key.F2) 
                    {
                        //to prevent "Round 0 ended" before the game actually began
                        if(roundCount > 0)
                            mainWindow.SendSemaineMessage(string.Format("Round {0} ended", roundCount));

                        roundCount++;
                    }

                    //left control + F3 means that the fly has been killed:
                    else if (args.Key == Key.F3)
                    {
                        mainWindow.SendSemaineMessage(string.Format("Killed fly in round {0}", roundCount));
                    }

                    //left control + F4 means that the fly has been missed:
                    else if (args.Key == Key.F4)
                    {
                        mainWindow.SendSemaineMessage(string.Format("Missed fly in round {0}", roundCount));
                    }

                    else if (args.Key == Key.F5)
                    {
                        mainWindow.SendSemaineMessage("Game has been started.");
                        Reset();
                    }

                    else if (args.Key == Key.F6)
                    {
                        mainWindow.SendSemaineMessage("Game has been closed.");
                    }

                    //since the combinations consist of two keys, "dispatch" the current combination and wait for a left control again
                    keyCombinationIsOpen = false;
                }
            }
        }
    }
}
