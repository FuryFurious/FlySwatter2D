using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SemaineApi.Components;
using SemaineApi.NMS.Message;
using SemaineApi.NMS.Receiver;
using SemaineApi.NMS.Sender;

namespace IGF_Semaine_Wrapper
{
    public delegate void DelProcessSemaineIncoming(object sender, EventArgsForSemaineIncoming e);

    //semaine event arguments class
    public class EventArgsForSemaineIncoming : EventArgs
    {
        public string message;
    }

    class SemaineClient : Component
    {
        // private Sender _sender;
        //  private Receiver _receiver;
        //  private Sender _sender_2;
        //  private Receiver _receiver_2;
        //  private Receiver _receiver_3;
        //private int _delay = 3000;
        //private long _lastMSG = 0;

        //semaine event
        public event DelProcessSemaineIncoming localEventObject;

        public SemaineClient(string own_desc, List<string> sender, List<string> receiver)
            : base(own_desc)
        {

            foreach (string send in sender)
            {
                string send_ = "semaine.data." + send;
                Sender _sender = new Sender(send_, "TEXT", own_desc);
                _senders.AddLast(_sender);
            }

            foreach (string rec in receiver)
            {
                string rec_ = "semaine.data." + rec;
                Receiver _receiver = new Receiver(rec_);
                _receivers.AddLast(_receiver);
            }

        }

        public string sendMsg(string str, int sender_id = 0)
        {
            try
            {
                Sender to_send = _senders.ElementAt(sender_id);
                to_send.SendTextMessage(str, _meta.CurrentTime);
            }
            catch (Apache.NMS.IllegalStateException ex)
            {
                if (ex.Message == "Connection is not started!")
                    return "notReady";
                return ex.GetType().FullName + ":" + ex.Message;
            }
            catch (Apache.NMS.NMSException ex)
            {
                if (ex.Message == "Exception Listener has received an exception, will not try to send")
                    return "notAccessibleAnymore";
                return ex.GetType().FullName + ":" + ex.Message;
            }
            catch (Exception ex)
            {
                return ex.GetType().FullName + ":" + ex.Message;
            }
            //_lastMSG = _meta.CurrentTime;
            return "1";
        }
        protected override void Act()
        {
        }

        protected override void React(SEMAINEMessage message)
        {
            //semaine event, all lines in this function
            EventArgsForSemaineIncoming tempEvent = new EventArgsForSemaineIncoming();
            tempEvent.message = message.Text;
            localEventObject(this, tempEvent);
            tempEvent = null;
        }

    }
}
