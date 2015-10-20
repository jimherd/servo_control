//
// Program to test communication between C# and MBED unit
//
// 
//
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.IO.Ports;     // added to use serial port features
using System.Threading;    // added to use sleep feature


namespace servo_control {
    public partial class Form1 : Form {

    //***********************************************************
    // Constant definitions
    //***********************************************************
        const int OK           =  0;   // some status return constants
        const int FORMAT_ERROR = -1;

        const int COM_BAUD     = 115200;
        const int READ_TIMEOUT = 10000;   // timeout for a read reply (10 seconds)

    //***********************************************************
    // global variables
    //***********************************************************
        int global_error;              // global variable for form1

        public Form1() {
            InitializeComponent();
        }

        //***********************************************************
        // User written functions
        //***********************************************************
        // get_reply : Read a status reply from the MBED
        //
        // All commands will return a status reply on completing the specified command.
        // A returned valur of 0 will indicate that all is well
        //
        public int get_reply() {

            string reply;
            int status;

            serialPort1.DiscardInBuffer();
            serialPort1.ReadTimeout = READ_TIMEOUT;
            try {
                reply = serialPort1.ReadLine();
            }
            catch (TimeoutException) {
                Debug_window.AppendText("Readline timeout fail" + Environment.NewLine);
                return -1;
            }
            status = Convert.ToInt32(reply);
            Debug_window.AppendText("Status = " + reply + Environment.NewLine);
            return status;
        }

    //***********************************************************
    // Functions that react to system events
    //***********************************************************
    //
    // Exit facility on form dropdown menu
    //
        private void exitToolStripMenuItem_Click(object sender, EventArgs e) {
            serialPort1.Close();
            this.Close();
        }

    //
    // Executed when form is first loaded. Main activity is to get a list of the available COM ports
    // on the computer and add them to the dropdown box (comboBox1).
    //
        private void Form1_Load(object sender, EventArgs e) {
            comboBox1.Items.Clear();
            foreach (string s in SerialPort.GetPortNames()) {
                comboBox1.Items.Add(s);
            }
            comboBox1.SelectedIndex = 0;
            serialPort1.BaudRate = COM_BAUD;
            global_error = OK;
            Thread.Sleep(2000);   // sleep 2 seconds
        }

    //
    // Try to open the COM port selected from the drop-down box (combobox)
    //
        private void button2_Click(object sender, EventArgs e) {
            string com_port = comboBox1.SelectedItem.ToString();
            Debug_window.AppendText("Selected COM Port = " + com_port + Environment.NewLine);
            try {
                Debug_window.AppendText("Trying to open " + com_port + Environment.NewLine);
                serialPort1.PortName = com_port;
                serialPort1.Open();
            }
            catch {
                Debug_window.AppendText("Cannot open " + com_port + Environment.NewLine);
                return;
            }
            Debug_window.AppendText(com_port + " now open" + Environment.NewLine);
        }

    //
    // send 's' servo command to MBED
    //
        private void button3_Click(object sender, EventArgs e) {
            string command = "s "
                           + Convert.ToString(numericUpDown1.Value) 
                           + " "
                           + Convert.ToString(numericUpDown2.Value);
            Debug_window.AppendText(command +  Environment.NewLine);
            serialPort1.WriteLine(command);
            Debug_window.AppendText("Reading reply" + Environment.NewLine);
            int status = get_reply();
        }

    //
    // send 'r' read command to MBED
    //
        private void button1_Click(object sender, EventArgs e) {
            String command = "r";
            Debug_window.AppendText(command + Environment.NewLine);
            serialPort1.WriteLine(command);
            Debug_window.AppendText("Reading reply" + Environment.NewLine);
            int status = get_reply();
            //
            // only read data if status reply was 0 (i.e. was successful)
            //
            if (status == 0) {
                Debug_window.AppendText("reading data" + Environment.NewLine);
                string data = serialPort1.ReadLine();
                Debug_window.AppendText("Data = " + data + Environment.NewLine);
            }
        }
//
// 'about' facility on form dropdown menu
// 
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e) {
            MessageBox.Show("Program to test C# to MBED comms.","About");
        }
    }
}
