using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsGraph
{
    public class BayesNode
    {
        string Name;
        public List<BayesNode> parents;
        public List<BayesNode> children;
        public Dictionary<string, double> probabTable;
        double init_true, init_false;
        double true_prob, false_prob;
        double tmp_true, tmp_false;
        public bool is_fixated;
        public bool is_visited;
        public int x;
        public int y;
        public int width;
        public int height;

        public BayesNode(string name)
        {
            this.Name = name;
            parents = new List<BayesNode>();
            children = new List<BayesNode>();
            probabTable = new Dictionary<string, double>();
            true_prob = false_prob = tmp_true = tmp_false = init_true = init_false = 0.0;
            is_fixated = false;
        }

        public double getTrueProb()
        {
            return true_prob;
        }
        public double getFalseProb()
        {
            return false_prob;
        }

        public bool hasParents()
        {
            if (this.parents.Count != 0)
                return true;
            return false;
        }

        public bool hasChildren()
        {
            if (this.children.Count != 0)
                return true;
            return false;
        }

        public void setChild(BayesNode child)
        {
            if (child != this)
            {
                children.Add(child);
                child.setParent(this);
            }
        }

        void setParent(BayesNode parent)
        {
            parents.Add(parent);
        }

        public string getName()
        {
            return this.Name;
        }

        public string showAllChindren()
        {
            string allChildren = "";

            if (this.hasChildren())
            {             
                allChildren += "Children of " + this.Name + " : ";

                foreach (BayesNode child in children)
                {
                    allChildren += child.getName() + "; ";
                }                
            }

            return allChildren;
        }

        public string showAllParents()
        {
            string allParents = "";

            if (this.hasParents())
            {
                allParents += "Parents of " + this.Name + " : ";

                foreach (BayesNode parent in parents)
                {
                    allParents += parent.getName() + "; ";
                }
            }

            return allParents;
        }


        public void setProbabTable(string key, double value)
        {
            probabTable.Add(key, value);
        }


        public void resetProbab()
        {
            if (!hasParents())
            {
                init_true = true_prob = tmp_true = 0.0;
                init_false = false_prob = tmp_false = 0.0;

                foreach (BayesNode child in children)
                {
                    child.resetProbab();
                    child.is_fixated = false;
                    child.is_visited = false;
                }
            }
            else
            {
                init_true = true_prob = tmp_true = 0.0;
                init_false = false_prob = tmp_false = 0.0;
            }

            is_fixated = false;
            is_visited = false;
        }

        public void setInitialProbability()
        {
            if (!hasParents())
            {
                init_true = true_prob = tmp_true = probabTable[this.Name + "True;"];
                init_false = false_prob = tmp_false = probabTable[this.Name + "False;"];

                foreach (BayesNode child in children)
                {
                    child.setInitialProbability();
                }
            }
            else
            {
                // розрахунок початкових ймовірностей
                foreach (BayesNode parent in parents)
                {
                    true_prob += probabTable[this.Name + "True;" + parent.getName() + "True;"] * parent.probabTable[parent.getName() + "True;"];
                    true_prob += probabTable[this.Name + "True;" + parent.getName() + "False;"] * parent.probabTable[parent.getName() + "False;"];
                }

                false_prob = 1 - true_prob;

                init_true = true_prob;
                init_false = false_prob;

            }
        }



        public void newChildrenProbab()
        {
            if (!is_fixated)
            {
                if (!is_visited)
                {
                    foreach (BayesNode parent in parents)
                    {
                        tmp_true += probabTable[this.Name + "True;" + parent.getName() + "True;"] * parent.tmp_true;
                        tmp_true += probabTable[this.Name + "True;" + parent.getName() + "False;"] * parent.tmp_false;
                    }

                    tmp_false = 1 - tmp_true;
                    is_visited = true;
                }
            }
        }

        public void countNewProbabForChildren()
        {
            if (!is_fixated)
            {
                if (!is_visited)
                {
                    if (this.hasParents())
                    {
                        foreach (BayesNode parent in parents)
                        {
                            tmp_true += probabTable[this.Name + "True;" + parent.getName() + "True;"] * parent.true_prob;
                            tmp_true += probabTable[this.Name + "True;" + parent.getName() + "False;"] * parent.false_prob;
                        }

                        tmp_false = 1 - tmp_true;
                        is_visited = true;

                        foreach (BayesNode parent in parents)
                        {
                            parent.countNewProbabForParents();
                        }
                    }
                }
            }
        }


        public void countNewProbabForParents()
        {
            if (!is_fixated && !is_visited)
            {
                if (this.hasChildren())
                {
                    double chislit = 0.0;
                    double znemenat = 0.0;

                    foreach (BayesNode child in children)
                    {
                        if (child.is_fixated)
                        {
                            if (child.true_prob == 1)
                            {
                                znemenat = child.tmp_true;
                            }
                            else
                            {
                                znemenat = child.tmp_false;
                            }

                            chislit += child.probabTable[child.getName() + "True;" + this.Name + "True;"] * probabTable[this.Name + "True;"] * child.true_prob;
                            chislit += child.probabTable[child.getName() + "True;" + this.Name + "False;"] * probabTable[this.Name + "False;"] * child.false_prob;
                        }

                    }

                    tmp_true = chislit / znemenat;

                    tmp_false = 1 - tmp_true;

                    is_visited = true;
                }


                // обчислення зміни ймовірності дочірніх вузлів по відношенню до батьківського вузла
                foreach (BayesNode child in children)
                {
                    if (!child.is_fixated)
                    {
                        child.newChildrenProbab();
                    }
                }

            }
        }


        public double changeProbability(bool state)
        {
            double res = 0.0;
            string stateSTR = Convert.ToString(state);
            stateSTR.ToUpper();
            string strMy = this.Name + stateSTR + ";";
            string strMyN = this.Name;


            this.is_fixated = true;
            this.is_visited = true;

            tmp_true = true_prob;
            tmp_false = false_prob;

            if (state == true)
            {
                true_prob = 1.0;
                false_prob = 0.0;
                strMyN += "False;";
            }
            else
            {
                true_prob = 0.0;
                false_prob = 1.0;
                strMyN += "True;";
            }

            bool test = hasDataInTable(strMy);
            if (test)
            {
                res = getTableValueByKey(strMy);

                if (!this.hasParents())
                {
                    foreach (BayesNode child in children)
                    {
                        child.countNewProbabForChildren();
                    }
                    foreach (BayesNode child in children)
                    {
                        child.FinalProbab();
                    }
                }
                if (!this.hasChildren())
                {
                    foreach (BayesNode parent in parents)
                    {
                        parent.countNewProbabForParents();
                    }
                    foreach (BayesNode parent in parents)
                    {
                        parent.FinalProbab();
                    }
                }

                if (!this.hasParents())
                {
                    foreach (BayesNode child in children)
                    {
                        child.is_visited = false;
                        child.FinalProbab();
                    }
                }
                if (!this.hasChildren())
                {
                    foreach (BayesNode parent in parents)
                    {
                        parent.is_visited = false;
                        parent.FinalProbab();
                    }
                }

            }
            else
            {
                res = 0.0;
            }

            return res;
        }

        public bool hasDataInTable(string key)
        {
            foreach (string keys in probabTable.Keys)
            {
                if (keys.Contains(key))
                    return true;
            }

            return false;
        }

        public double getTableValueByKey(string key)
        {
            double data = 0.0;


            foreach (string keys in probabTable.Keys)
            {
                if (key.Contains(key))
                {
                    return data = probabTable[keys];
                }
            }

            return data;
        }


        public void FinalProbab()
        {
            if (this.hasChildren())
            {
                foreach (BayesNode child in children)
                {
                    child.true_prob = child.tmp_true;
                    child.false_prob = child.tmp_false;
                }
            }
            if (this.hasParents())
            {
                foreach (BayesNode parent in parents)
                {
                    parent.true_prob = parent.tmp_true;
                    parent.false_prob = parent.tmp_false;
                }
            }

            true_prob = tmp_true;
            false_prob = tmp_false;
        }

    }

}
