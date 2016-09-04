using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using PySynCS.Translator;

namespace PySynCS.GUI {
    public partial class MainGUI : Form {

        MessyTranslator trans;
        PySynCS.Translator.Settings.TranslatorSettings settings;

        public MainGUI() {
            InitializeComponent();

            settings                 = new Translator.Settings.TranslatorSettings();
            settings.SyntaxType      = ESyntaxType.Python_Tabs;
            settings.TranslationType = ETranslationType.toPySynCS;

            trans = new MessyTranslator(settings);
        }

        private void button1_Click(object sender, EventArgs e) {
            richTextBox3.Text = "";
            richTextBox2.Text = trans.TO_PYSYNCS_TranslateViaString(richTextBox1.Text, this);
        }

        public void WriteDebug(string x) {
            richTextBox3.AppendText(x + "\n");
        }
    }
}
