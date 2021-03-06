﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using PySynCS.Translator;
using PySynCS.Translator.Cache;

namespace PySynCS.GUI {
    public partial class MainGUI : Form {

        Translator.Translator                           trans;
        PySynCS.Translator.Settings.TranslatorSettings  settings;
        Translator.Logger.Logger                        logger;


        public MainGUI() {
            InitializeComponent();

            // Setup the translator settings
            settings                 = new Translator.Settings.TranslatorSettings();
            settings.SyntaxType      = ESyntaxType.Python_Tabs;
            settings.TranslationType = ETranslationType.toPySynCS;

            // Setup the translator
            trans                    = new EvenMessierTranslator(settings);

            // Setup the logger
            logger = new Translator.Logger.Logger();
            logger.Debug.evtClear       += Logger_evtClear;
            logger.Debug.evtWrite       += Logger_evtWrite;
            logger.Debug.evtWriteLine   += Logger_evtWriteLine;
        }

        private void Logger_evtWriteLine(Translator.Logger.BaseLogger log, Translator.Translator t, Cache cache, string str) {
            richTextBox3.AppendText($"[{log.Name} - {cache.CurrentLine}]: {str}\n");
        }

        private void Logger_evtWrite(Translator.Logger.BaseLogger log, Translator.Translator t, Cache cache, string str) {
            richTextBox3.AppendText(str);
        }

        private void Logger_evtClear(Translator.Logger.BaseLogger log, Translator.Translator t, Cache cache) {
            richTextBox3.Clear();
        }
        
        private void button1_Click(object sender, EventArgs e) {
            richTextBox3.Text = "";

            // Create Logger
            richTextBox2.Text = trans.ToPySyncsFromString(richTextBox1.Text, this.logger);
        }
    }
}
