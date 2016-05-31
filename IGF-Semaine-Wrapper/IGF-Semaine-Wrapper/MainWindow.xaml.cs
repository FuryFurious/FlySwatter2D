using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using SemaineApi.System;
using System.Windows.Threading;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.IO;

namespace IGF_Semaine_Wrapper
{
    /// <summary>
    /// Programm: IGF-Semaine-Wrapper-Tool
    /// Autor: Patrick Hühne
    /// Jahr: 2016
    /// </summary>
    public partial class MainWindow : Window
    {
        KeyboardListener kListener = new KeyboardListener();
        //XML-Files
        string config_xmlFile = @"Konfiguration/konfiguration.xml";
       // string ressources_xmlFile = @"Konfiguration/ressourcen.xml";

        //Semaine Connection Values
        SemaineClient sc;
        //bool systemmanager_is_connected = false;

        private MyEventReceiver eventReceiver;


        //Wrapping Instructions
        List<String[]> list_wrapping_ins = new List<string[]>();

        private bool combinationIsOpen;

        int roundCount = 0;

        public MainWindow()
        {
            InitializeComponent();

            eventReceiver = new MyEventReceiver(kListener, this);

            init_lists();
            connect_to_semaine();
        }


        private void connect_to_semaine()
        {
            try
            {
                XDocument xdocument = XDocument.Load(config_xmlFile);
                //loading xml data in the program
                XElement xe = xdocument.Element("Configuration");
                XElement xetemp = xe.Element("Semaine");

                ComponentRunner runner = new ComponentRunner(@"Konfiguration/sfb.config.xml");
                runner.Go();
                System.Threading.Thread.Sleep(250);


                List<string> semaineIncoming = new List<string>();
                XElement xetemp_2 = xetemp.Element("incomingToCU");
                init_list(semaineIncoming, xetemp_2, "adress_name");

                List<string> semaineOutgoing = new List<string>();
                xetemp_2 = xetemp.Element("outgoingFromCU");
                init_list(semaineOutgoing, xetemp_2, "adress_name");


                this.sc = new SemaineClient("IGF_Semaine_Wrapper", semaineIncoming, semaineOutgoing);
                this.sc.Start();
                this.sc.localEventObject += EventHandlerProcessSemaineIncoming;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Es konnte keine Verbindung zum Semaine Systemmanager hergestellt werden. Wurde der Systemmangaer gestartet? Stimmt die eingetragene IP-Adresse unter Konfiguration/sfb.config.xml?");
              /*  this.lab_connect_info_sm.Background = Brushes.Red;
                this.lab_connect_info_cu.Background = Brushes.Red;
                this.lab_connect_info_client.Background = Brushes.Red;*/
            }
        }


        private void init_lists()
        {
            XDocument xdocument = XDocument.Load(config_xmlFile);
            XElement temp = xdocument.Element("Configuration");
            string[] str_sys_ques_att = { "input_format", "input_data", "output_format", "output_data", "output_sender" };
            init_list_with_ids(this.list_wrapping_ins, temp, "Wrapper_Instructions", "wrapper_ins", str_sys_ques_att);
        }

        #region Semaine_Work

        public void EventHandlerProcessSemaineIncoming(object sender, EventArgsForSemaineIncoming e)
        {
           

            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (e.message.Contains("cu.own.hash:"))
                {

                }
                else if (e.message == "cu.own.ping")
                {
                    SendSemaineMessage("IGF_Semaine_Wrapper: pinged");
                }
                else if (e.message == "cu.own.start")
                {
                    SendSemaineMessage("IGF_Semaine_Wrapper: started");
                }

                try
                {
                    if (!e.message.Contains("broadcast"))
                    {
                        foreach (String[] str_arr in this.list_wrapping_ins)
                        {

                            if (e.message.Contains(str_arr[1]))
                            {
                                SendSemaineMessage(str_arr[3], Convert.ToInt32(str_arr[4]));
                            }
                        }
                    }

                }catch (Exception ex)
                {
                    MessageBox.Show("Fehler beim wrappen. Bitte XML prüfen. Fehler: \n"+ ex.Message);
                }

            }));
        }

        public void SendSemaineMessage(string message_to_send, int output_channel = 0)
        {
            if (this.sc != null)
            {
                string erg = this.sc.sendMsg(message_to_send, output_channel);
                Label lab_to_add = new Label();
                DateTime time = DateTime.Now;

                lab_to_add.Content = string.Concat(time.ToString("g"), "\t" , message_to_send, "\t an Senderkanal: ", Convert.ToString(output_channel));
                this.sP_debug.Children.Add(lab_to_add);
            }
        }

        #endregion

        #region Helper_Functions
        public void init_list(List<string> to_init, XElement parent_element, string xelement_name)
        {
            foreach (XElement xel in parent_element.Elements(xelement_name))
            {
                to_init.Add(xel.Value);
                // MessageBox.Show(xel.Value);
            }
        }

        //Init Function 2 for Filelists
        public void init_list_with_ids(List<string[]> to_init, XElement parent_element, string xelement_name, string xelement_child, string[] xelements_values)
        {
            if (parent_element != null)
            {
                var elements = parent_element.Elements(xelement_name);
                if (!xelement_child.Equals("xxx0x"))
                    elements = parent_element.Elements(xelement_name).Elements(xelement_child);

                foreach (XElement xel in elements)
                {
                    string[] temp_to_add = new string[xelements_values.Length];
                    for (int i = 0; i < xelements_values.Length; i++)
                    {
                        // MessageBox.Show(xel.Element(xelements_values[i]).Value);
                        temp_to_add[i] = xel.Element(xelements_values[i]).Value;
                    }

                    to_init.Add(temp_to_add);
                    //   MessageBox.Show(temp_to_add[0]);
                }
            }
        }
        #endregion

        private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            ScrollViewer scroll = (ScrollViewer)sender;
            scroll.ScrollToEnd();
        }



    }
}
