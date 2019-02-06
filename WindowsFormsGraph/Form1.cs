using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsGraph
{
    public partial class Form1 : Form
    {
        BayesNode Alcohol;
        BayesNode Cirrhosis;
        BayesNode Steatosis;
        BayesNode Hemochromatosis;
        BayesNode Fibrosis;
        BayesNode Hepatitis;
        List<BayesNode> ParentNodes;



        public Form1()
        {
            InitializeComponent();

            InitNodes();

            Bitmap bmp = new Bitmap(9999, 9999);
            pictureBox1.SizeMode = PictureBoxSizeMode.AutoSize;
            pictureBox1.Image = bmp;
            

        }        

        private void Form1_Load(object sender, EventArgs e)
        {
            InitNodes();
            drawNodeRect();
            drawNodeDependLines();
            createFormElements();
            setAllRadiobuttonsEvent();
        }        


        // Инициализация узлов и их таблиц и их отрисовка
        public void InitNodes()
        {
            Alcohol = new BayesNode("Алкоголізм");
            Cirrhosis = new BayesNode("Цироз");
            Steatosis = new BayesNode("Стеатоз");
            Hemochromatosis = new BayesNode("Гемохроматоз");
            Fibrosis = new BayesNode("Фіброз");
            Hepatitis = new BayesNode("Гепатит");

            Alcohol.setChild(Steatosis);
            Alcohol.setChild(Cirrhosis);
            Alcohol.setChild(Hemochromatosis);
            Alcohol.setChild(Fibrosis);
            Alcohol.setChild(Hepatitis);

            Alcohol.setProbabTable("АлкоголізмTrue;", 0.265);
            Alcohol.setProbabTable("АлкоголізмFalse;", 0.735);

            Steatosis.setProbabTable("СтеатозTrue;АлкоголізмTrue;", 0.65);
            Steatosis.setProbabTable("СтеатозFalse;АлкоголізмTrue;", 0.35);
            Steatosis.setProbabTable("СтеатозTrue;АлкоголізмFalse;", 0.26);
            Steatosis.setProbabTable("СтеатозFalse;АлкоголізмFalse;", 0.74);

            Cirrhosis.setProbabTable("ЦирозTrue;АлкоголізмTrue;", 0.86);
            Cirrhosis.setProbabTable("ЦирозFalse;АлкоголізмTrue;", 0.14);
            Cirrhosis.setProbabTable("ЦирозTrue;АлкоголізмFalse;", 0.2);
            Cirrhosis.setProbabTable("ЦирозFalse;АлкоголізмFalse;", 0.6);

            Hemochromatosis.setProbabTable("ГемохроматозTrue;АлкоголізмTrue;", 0.333);
            Hemochromatosis.setProbabTable("ГемохроматозFalse;АлкоголізмTrue;", 0.667);
            Hemochromatosis.setProbabTable("ГемохроматозTrue;АлкоголізмFalse;", 0.1);
            Hemochromatosis.setProbabTable("ГемохроматозFalse;АлкоголізмFalse;", 0.9);

            Fibrosis.setProbabTable("ФіброзTrue;АлкоголізмTrue;", 0.1);
            Fibrosis.setProbabTable("ФіброзFalse;АлкоголізмTrue;", 0.9);
            Fibrosis.setProbabTable("ФіброзTrue;АлкоголізмFalse;", 0.45);
            Fibrosis.setProbabTable("ФіброзFalse;АлкоголізмFalse;", 0.55);

            Hepatitis.setProbabTable("ГепатитTrue;АлкоголізмTrue;", 0.3);
            Hepatitis.setProbabTable("ГепатитFalse;АлкоголізмTrue;", 0.7);
            Hepatitis.setProbabTable("ГепатитTrue;АлкоголізмFalse;", 0.1);
            Hepatitis.setProbabTable("ГепатитFalse;АлкоголізмFalse;", 0.9);

            Alcohol.setInitialProbability();


            ParentNodes = new List<BayesNode>();
            ParentNodes.Add(Alcohol);

            drawNodeRect();
        }


        public void drawNodeRect()
        {
            List<BayesNode> usedChild = new List<BayesNode>();


            int x = 10, y = 45;
            int width = 160, height = 70;
            int cX = x, cY = y + height + 50;

            foreach (BayesNode node in ParentNodes)
            {
                drawRect(x, y, width, height);
                node.x = x; node.y = y;
                node.height = height;
                node.width = width;


                int pX = x, pY = y + height + 50;

                if (node.hasChildren())
                {
                    foreach (BayesNode child in node.children)
                    {

                        if (!usedChild.Contains(child))
                        {

                            usedChild.Add(child);

                            drawRect(pX, pY, width, height);
                            child.x = pX; child.y = pY;
                            child.height = height;
                            child.width = width;

                            pX += width + 20;
                        }
                    }
                }

                x += width + 20;
            }
        }


        // отрисовка связей между узлами
        public void drawNodeDependLines()
        {
            List<BayesNode> usedChild = new List<BayesNode>();

            foreach (BayesNode node in ParentNodes)
            {
                if (node.hasChildren())
                {
                    foreach(BayesNode child in node.children)
                    {
                        if (!usedChild.Contains(child))
                        {

                            usedChild.Add(child);

                            int childCentr = 1;
                            if ((child.width % 2) == 0)
                            {
                                childCentr = child.width / 2;
                            }
                            else
                            {
                                childCentr = child.width + 1;
                                childCentr /= 2;
                            }

                            foreach (BayesNode chilPar in child.parents)
                            {
                                int parCentr;
                                if ((chilPar.width % 2) == 0)
                                {
                                    parCentr = chilPar.width / 2;
                                }
                                else
                                {
                                    parCentr = chilPar.width + 1;
                                    parCentr /= 2;
                                }


                                drawLines(child.x + childCentr, child.y, chilPar.x + parCentr, chilPar.y + chilPar.height);
                            }
                        }
                    }
                }
            }

        }

        // рисование линий
        public void drawLines(int X1, int Y1, int X2, int Y2)
        {
            Graphics formGraphics;
            formGraphics = pictureBox1.CreateGraphics();
            Pen myPen = new Pen(Color.Black, 1);
            formGraphics.DrawLine(myPen, X1, Y1, X2, Y2);
        }

        // рисование прямоугольников
        public void drawRect(int X, int Y, int Width, int Height)
        {
            Graphics formGraphics;
            formGraphics = pictureBox1.CreateGraphics();
            Pen p = new Pen(Color.Black, 2);
            formGraphics.DrawRectangle(p, X, Y, Width, Height);
        }

        // отрисовка элементов форм
        public void createFormElements()
        {
            int Nom = 0;
            List<BayesNode> usedChild = new List<BayesNode>();

            foreach (BayesNode node in ParentNodes)
            {
                if (node.hasChildren())
                {
                    foreach (BayesNode child in node.children)
                    {
                        Nom++;
                    }
                }

                Nom++;
            }


            Label[] labels = new Label[Nom];
            RadioButton[] trueRadios = new RadioButton[Nom];
            RadioButton[] falseRadios = new RadioButton[Nom];

            int i = 0;
            foreach (BayesNode node in ParentNodes)
            {
                Point pp = new Point();
                pp.X = node.x + 5;
                pp.Y = node.y + 5;
                labels[i] = new Label();
                labels[i].Text = node.getName();
                labels[i].Location = pp;

                trueRadios[i] = new RadioButton();
                trueRadios[i].Name = node.getName() + "True";
                pp.Y += 20;
                trueRadios[i].Location = pp;
                trueRadios[i].Text = "True: " + (node.getTrueProb() * 100).ToString("0.##") + " %";
                falseRadios[i] = new RadioButton();
                falseRadios[i].Name = node.getName() + "False";
                pp.Y += 20;
                falseRadios[i].Location = pp;
                falseRadios[i].Text = "False: " + (node.getFalseProb() * 100).ToString("0.##") +" %";

                i++;

                if (node.hasChildren())
                {
                    foreach (BayesNode child in node.children)
                    {
                        Point pch = new Point();
                        pch.X = child.x + 5;
                        pch.Y = child.y + 5;
                        labels[i] = new Label();
                        labels[i].Text = child.getName();
                        labels[i].Location = pch;

                        trueRadios[i] = new RadioButton();
                        trueRadios[i].Name = child.getName() + "True";
                        pch.Y += 20;
                        trueRadios[i].Location = pch;
                        trueRadios[i].Text = "True: " + (child.getTrueProb() * 100).ToString("0.##") + " %";

                        falseRadios[i] = new RadioButton();
                        falseRadios[i].Name = child.getName() + "False";
                        pch.Y += 20;
                        falseRadios[i].Location = pch;
                        falseRadios[i].Text = "False: " + (child.getFalseProb() * 100).ToString("0.##") + " %";

                        i++;
                    }
                }
            }


            for (int j = 0; j < Nom; j++)
            {
                pictureBox1.Controls.Add(labels[j]);
                pictureBox1.Controls.Add(trueRadios[j]);
                pictureBox1.Controls.Add(falseRadios[j]);
            }

        }


        public void setAllRadiobuttonsEvent()
        {
            foreach (RadioButton radioBtn in pictureBox1.Controls.OfType<RadioButton>())
            {
                radioBtn.Click += new EventHandler(radioButtons_ClickEvent);
            }
        }


        // событие при нажатии на RadioButton
        private void radioButtons_ClickEvent(object sender, EventArgs e)
        {
            RadioButton rb = sender as RadioButton;

            if (rb != null)
            {
                string rbName = rb.Name;
                bool trig = false;
                if(rbName.Contains("True"))
                {
                    trig = true;
                }
                if(rbName.Contains("False"))
                {
                    trig = false;
                }                
                rbName = rbName.Replace(trig.ToString(), "");

                // отправляем имя узла и какую переменую выставляем
                DoSomething(rbName, trig);
            }
        }


        public void DoSomething(string name, bool triger)
        {
            foreach(BayesNode node in ParentNodes)
            {
                node.resetProbab();
                node.setInitialProbability();
            }

            string sender = name + triger;
            bool oposite = !triger;
            string oposSender = name + oposite;

            foreach(BayesNode node in ParentNodes)
            {
                if (node.getName() == name)
                {
                    node.changeProbability(triger);
                }
                else
                {
                    foreach (BayesNode child in node.children)
                    {
                        if (child.getName() == name)
                        {
                            child.changeProbability(triger);
                        }
                    }
                }

            }

            updateRadiobuttonsText(sender, oposSender);
        }

        

        // Обновление текста в radiobutton всех узлов
        public void updateRadiobuttonsText(string sender, string oposite_sender)
        {
            List<BayesNode> usedChild = new List<BayesNode>();
            
            List<RadioButton> allRadioTrue = new List<RadioButton>();
            List<RadioButton> allRadioFalse = new List<RadioButton>();

            foreach (RadioButton radioBtn in pictureBox1.Controls.OfType<RadioButton>())
            {
                if(radioBtn.Name.Contains("True"))
                {
                    allRadioTrue.Add(radioBtn);
                }
                if (radioBtn.Name.Contains("False"))
                {
                    allRadioFalse.Add(radioBtn);
                }
            }


            foreach(BayesNode node in ParentNodes)
            {
                foreach(RadioButton radT in allRadioTrue)
                {
                    if (radT.Name.Contains(node.getName()))
                    {
                        radT.Text = "True : " + (node.getTrueProb() * 100).ToString("0.##") + " %";
                    }
                }
                foreach(RadioButton radF in allRadioFalse)
                {
                    if (radF.Name.Contains(node.getName()))
                    {
                        radF.Text = "False : " + (node.getFalseProb() * 100).ToString("0.##") + " %";
                    }
                }


                if (node.hasChildren())
                {
                    foreach (BayesNode child in node.children)
                    {

                        if (!usedChild.Contains(child))
                        {
                            usedChild.Add(child);

                            foreach (RadioButton radT in allRadioTrue)
                            {
                                if (radT.Name.Contains(child.getName()))
                                {
                                    radT.Text = "True : " + (child.getTrueProb() * 100).ToString("0.##") + " %";
                                }
                            }
                            foreach (RadioButton radF in allRadioFalse)
                            {
                                if (radF.Name.Contains(child.getName()))
                                {
                                    radF.Text = "False : " + (child.getFalseProb() * 100).ToString("0.##") + " %";
                                }
                            }

                        }
                    }
                }                
            }



            foreach (RadioButton radioBtn in pictureBox1.Controls.OfType<RadioButton>())
            {
                if(radioBtn.Name == sender)
                {
                    string state = "";
                    if (sender.Contains("True")) state = "True";
                    else state = "False";
                    radioBtn.Text = state + ": 100%";
                }
                if(radioBtn.Name == oposite_sender)
                {
                    string state = "";
                    if (oposite_sender.Contains("True")) state = "True";
                    else state = "False";
                    radioBtn.Text = state + ": 0%";
                }
            }

        }

        

        public void resetAllValues()
        {
            List<RadioButton> allRadio = new List<RadioButton>();

            foreach (RadioButton radioBtn in pictureBox1.Controls.OfType<RadioButton>())
            {
                allRadio.Add(radioBtn);
                radioBtn.Checked = false;
            }


            
            foreach (BayesNode node in ParentNodes)
            {
                foreach(RadioButton rbtn in allRadio)
                {
                    if(rbtn.Name.Contains(node.getName()))
                    {
                        if(rbtn.Name.Contains("True"))
                        {
                            rbtn.Text = "True: " + (node.getTrueProb() * 100).ToString("0.##") + " %";
                        }
                        if(rbtn.Name.Contains("False"))
                        {
                            rbtn.Text = "False: " + (node.getFalseProb() * 100).ToString("0.##") + " %";
                        }
                    }
                }

                if (node.hasChildren())
                {
                    foreach (BayesNode child in node.children)
                    {

                        foreach (RadioButton rbtn in allRadio)
                        {
                            if (rbtn.Name.Contains(child.getName()))
                            {
                                if (rbtn.Name.Contains("True"))
                                {
                                    rbtn.Text = "True: " + (child.getTrueProb() * 100).ToString("0.##") + " %";
                                }
                                if (rbtn.Name.Contains("False"))
                                {
                                    rbtn.Text = "False: " + (child.getFalseProb() * 100).ToString("0.##") + " %";
                                }
                            }
                        }
                    }
                }
            }
        }


        private void buttonReset_Click(object sender, EventArgs e)
        {
            foreach (BayesNode node in ParentNodes)
            {
                node.resetProbab();
                node.setInitialProbability();
            }

            resetAllValues();

        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            InitNodes();
            drawNodeRect();
            drawNodeDependLines();
        }

        private void panel1_Scroll(object sender, ScrollEventArgs e)
        {
            drawNodeRect();
            drawNodeDependLines();
        }

        private void pictureBox1_LoadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            InitNodes();
            drawNodeRect();
            drawNodeDependLines();
        }

        
    }
}
