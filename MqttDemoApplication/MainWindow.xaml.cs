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
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace MqttDemoApplication
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Attribuut
        MqttClient client = null;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Connect_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Connectie maken met broker ...");

            // Nieuw MqttClient object
            string hostAddress = Host.Text;
            client = new MqttClient(hostAddress);

            // Connectie maken met broker
            client.Connect(ClientID.Text);

            if (client.IsConnected)
            {
                Console.WriteLine("Connectie met broker gemaakt");
                Host.IsEnabled = false;
                ClientID.IsEnabled = false;
                Connect.IsEnabled = false;
                Disconnect.IsEnabled = true;
                Publish.IsEnabled = true;
                Subscribe.IsEnabled = true;
            }
            else
            {
                Console.WriteLine("Connectie met broker gefaald.");
            }
        }

        private void Disconnect_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Connectie verbreken met broker");
            client.Disconnect();

            Host.IsEnabled = true;
            ClientID.IsEnabled = true;
            Connect.IsEnabled = true;
            Disconnect.IsEnabled = false;
            Publish.IsEnabled = false;
            Subscribe.IsEnabled = false;
        }

        private void Publish_Click(object sender, RoutedEventArgs e)
        {
            string topic = PublishTopic.Text;
            string message = PublishMessage.Text;

            if (client != null && client.IsConnected)
            {
                client.Publish(topic, Encoding.UTF8.GetBytes(message));
            }
            else
            {
                Console.WriteLine("Kan niet publishen zonder connectie");
            }
        }

        private void Subscribe_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Subscriben op topic");

            string topic = SubscribeTopic.Text;
            
            // Registreren / subscriben op topic
            client.Subscribe(new string[] { topic }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });

            // Handler registeren waar de berichten op terecht moeten komen
            client.MqttMsgPublishReceived += MqttMessageReceived;
        }

        private void MqttMessageReceived(object sender, MqttMsgPublishEventArgs e)
        {
            Console.WriteLine("Bericht ontvangen");

            // byte array omzetten naar string
            string bericht = Encoding.UTF8.GetString(e.Message);
            Console.WriteLine(bericht);

            // Update de GUI vanuit het MQTT process
            Dispatcher.Invoke(delegate
            {
                SubscribeMessage.Text = bericht;
            });
        }
    }
}
