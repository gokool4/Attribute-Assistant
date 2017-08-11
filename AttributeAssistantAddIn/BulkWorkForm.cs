using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.ADF;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Editor;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using A4LGSharedFunctions;

namespace ArcGIS4LocalGovernment
{
    public partial class BulkWorkForm : Form
    {
        IQueryFilter qFilter;
        public static string _sequenceTableName = "GenerateId";
        
        public BulkWorkForm()
        {
            InitializeComponent();
            GetConfigSettings();
            txtFerDia.Text = ConfigUtil.GetConfigValue("FerruleDiamater");//_enabledOnStart); (A4LGSharedFunctions.Localizer.GetString("FerruleDiamater"));
            txtFerMat.Text =  ConfigUtil.GetConfigValue("FerruleMaterial");
            txtFerSC.Text =  ConfigUtil.GetConfigValue("FerruleOwnedBy");
            txtFerJP.Text = ConfigUtil.GetConfigValue("FerruleJobPlan");
            txtTSDia.Text = ConfigUtil.GetConfigValue("TappingSaddleDiameter");
            txtTapMat.Text = ConfigUtil.GetConfigValue("TappingSaddleMaterial");
            txtTSSC.Text = ConfigUtil.GetConfigValue("TappingSaddleOwnedBy");
            txtTSJP.Text = ConfigUtil.GetConfigValue("TappingSaddleJobPlan");
            txtBBDia.Text = ConfigUtil.GetConfigValue("BBDiameter");
            txtBBMat.Text = ConfigUtil.GetConfigValue("BBMaterial");
            txtBBSC.Text = ConfigUtil.GetConfigValue("BBOwnedBy");
            txtBBJP.Text = ConfigUtil.GetConfigValue("BBJobPlan");
            txtESDia.Text = ConfigUtil.GetConfigValue("ESDiameter");
            txtESMat.Text = ConfigUtil.GetConfigValue("ESMaterial");
            txtESSC.Text = ConfigUtil.GetConfigValue("ESOwnedBy");
            txtESJP.Text = ConfigUtil.GetConfigValue("ESJobPlan");
            txtSPSDia.Text = ConfigUtil.GetConfigValue("SPSDiameter");
            txtSPSLengthBand.Text = ConfigUtil.GetConfigValue("SPSServiceLengthBand");
            txtSPSSS.Text = ConfigUtil.GetConfigValue("SPS");
            txtSPSJP.Text = ConfigUtil.GetConfigValue("SPSJobPlan");
            txtSPLDia.Text = ConfigUtil.GetConfigValue("SPLDiameter");
            txtSPLLengthBand.Text = ConfigUtil.GetConfigValue("SPLServiceLengthBand");
            txtSPLSS.Text = ConfigUtil.GetConfigValue("SPL");
            txtSPLJP.Text = ConfigUtil.GetConfigValue("SPLJobPlan");
            txtCMeterFitType.Text = ConfigUtil.GetConfigValue("CMMeterFitType");
            txtCMMeterSize.Text = ConfigUtil.GetConfigValue("CMMeterSize");
            txtCMMeterType.Text = ConfigUtil.GetConfigValue("CMMeterType");
            txtCMJP.Text = ConfigUtil.GetConfigValue("CMDomJobPlan");
            txtCMNonDomMeterFitType.Text = ConfigUtil.GetConfigValue("CMMeterFitType");
            txtCMNonDomMeterType.Text = ConfigUtil.GetConfigValue("CMMeterType");
            txtCMNonDomMeterSize.Text = ConfigUtil.GetConfigValue("CMMeterSize");
            txtCMNonDomJobPLan.Text = ConfigUtil.GetConfigValue("CMNonDomJobPlan");
            



            
        }



        private void btn_Submit_Click(object sender, EventArgs e)
        {
            try
            {

                bool FCorLayerTarget = true;
                bool boolLayerOrFC = true;
                string classItem;
                IList<IFeatureLayer> fLayerList = new List<IFeatureLayer>();




                try
                {
                    if (AAState._editor.SelectionCount <= Convert.ToInt16(ConfigUtil.GetConfigValue("FeatureCount")))
                    {

                        progressBar1.Maximum = AAState._editor.SelectionCount;
                        //progressBar1.Step = progress;
                        progressBar1.Value = 0;
                        AAState.PerformUpdates = false;

                        AAState._editor.StartOperation();
                        int i = 0;
                        double count = Convert.ToDouble(AAState._editor.SelectionCount);
                        IEnumFeature selection = AAState._editor.EditSelection;
                        IObject obj = selection.Next();
                        while (obj != null)
                        {
                            if (obj.Class.AliasName == ConfigUtil.GetConfigValue("DESIGNEDWPRN"))
                            {


                                if (rbFerrule.Checked)
                                {
                                    CreateFeature((Globals.FindLayer(ArcMap.Application, ConfigUtil.GetConfigValue("ServiceWorksPoint"), ref FCorLayerTarget) as IFeatureLayer).FeatureClass, (obj as IFeature), (obj as IFeature).Shape, ConfigUtil.GetConfigValue("Ferrule"),
                                        ConfigUtil.GetConfigValue("FerruleDiamater"), ConfigUtil.GetConfigValue("FerruleMaterial"), ConfigUtil.GetConfigValue("FerruleOwnedBy"), ConfigUtil.GetConfigValue("FerruleJobPlan"));
                                }
                                if (rbTappingSaddle.Checked)
                                {
                                    CreateFeature((Globals.FindLayer(ArcMap.Application, ConfigUtil.GetConfigValue("ServiceWorksPoint"), ref FCorLayerTarget) as IFeatureLayer).FeatureClass, (obj as IFeature), (obj as IFeature).Shape, ConfigUtil.GetConfigValue("TappingSaddle"),
                                        ConfigUtil.GetConfigValue("TappingSaddleDiameter"), ConfigUtil.GetConfigValue("TappingSaddleMaterial"), ConfigUtil.GetConfigValue("TappingSaddleOwnedBy"), ConfigUtil.GetConfigValue("TappingSaddleJobPlan"));
                                }
                                if (rbBoundaryBox.Checked)
                                {
                                    CreateFeature((Globals.FindLayer(ArcMap.Application, ConfigUtil.GetConfigValue("ServiceWorksPoint"), ref FCorLayerTarget) as IFeatureLayer).FeatureClass, (obj as IFeature), (obj as IFeature).Shape, ConfigUtil.GetConfigValue("BB"),
                                        ConfigUtil.GetConfigValue("BBDiameter"), ConfigUtil.GetConfigValue("BBMaterial"), ConfigUtil.GetConfigValue("BBOwnedBy"), ConfigUtil.GetConfigValue("BBJobPlan"));
                                }
                                if (rbExisiting.Checked)
                                {
                                    CreateFeature((Globals.FindLayer(ArcMap.Application, ConfigUtil.GetConfigValue("ServiceWorksPoint"), ref FCorLayerTarget) as IFeatureLayer).FeatureClass, (obj as IFeature), (obj as IFeature).Shape, ConfigUtil.GetConfigValue("ES"),
                                        ConfigUtil.GetConfigValue("ESDiameter"), ConfigUtil.GetConfigValue("ESMaterial"), ConfigUtil.GetConfigValue("ESOwnedBy"), ConfigUtil.GetConfigValue("ESJobPlan"));
                                }
                                if (rbPipeShort.Checked)
                                {
                                    CreateLineFeature((Globals.FindLayer(ArcMap.Application, ConfigUtil.GetConfigValue("lateralLine"), ref FCorLayerTarget) as IFeatureLayer).FeatureClass, (obj as IFeature), (obj as IFeature).Shape, ConfigUtil.GetConfigValue("SPS"),
                                        ConfigUtil.GetConfigValue("SPSDiameter"), ConfigUtil.GetConfigValue("SPSServiceLengthBand"), ConfigUtil.GetConfigValue("SPSOwnedBy"), ConfigUtil.GetConfigValue("SPSJobPlan"));
                                }
                                if (rbPipeLong.Checked)
                                {
                                    CreateLineFeature((Globals.FindLayer(ArcMap.Application, ConfigUtil.GetConfigValue("lateralLine"), ref FCorLayerTarget) as IFeatureLayer).FeatureClass, (obj as IFeature), (obj as IFeature).Shape, ConfigUtil.GetConfigValue("SPL"),
                                        ConfigUtil.GetConfigValue("SPLDiameter"), ConfigUtil.GetConfigValue("SPLServiceLengthBand"), ConfigUtil.GetConfigValue("SPLOwnedBy"), ConfigUtil.GetConfigValue("SPLJobPlan"));
                                }
                                if (rbCustomerMeterDom.Checked)
                                {
                                    CreateFeature((Globals.FindLayer(ArcMap.Application, ConfigUtil.GetConfigValue("CustomerMeter"), ref FCorLayerTarget) as IFeatureLayer).FeatureClass, (obj as IFeature), (obj as IFeature).Shape, ConfigUtil.GetConfigValue("CMMeterFitType"),
                                        ConfigUtil.GetConfigValue("CMMeterType"), ConfigUtil.GetConfigValue("CMMeterSize"), ConfigUtil.GetConfigValue("CMDOMCustomerType"), ConfigUtil.GetConfigValue("CMDomJobPlan"));
                                }
                                if (rbCustomerMeterNonDom.Checked)
                                {
                                    CreateFeature((Globals.FindLayer(ArcMap.Application, ConfigUtil.GetConfigValue("CustomerMeter"), ref FCorLayerTarget) as IFeatureLayer).FeatureClass, (obj as IFeature), (obj as IFeature).Shape, ConfigUtil.GetConfigValue("CMMeterFitType"),
                                        ConfigUtil.GetConfigValue("CMMeterType"), ConfigUtil.GetConfigValue("CMMeterSize"), ConfigUtil.GetConfigValue("CMNonDOMCustomerType"), ConfigUtil.GetConfigValue("CMNonDomJobPlan"));
                                }
                                //backgroundWorker.ReportProgress(++i);


                            }
                            ++i;

                            progressBar1.Value = i;

                            obj = selection.Next();
                        }
                        AAState.PerformUpdates = true;
                    }
                    else
                    {
                        MessageBox.Show(String.Format("The number of selected features is more than the configured limit of {0}", ConfigUtil.GetConfigValue("FeatureCount")));
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message + " , " + ex.StackTrace);

                }
                finally
                {
                    AAState._editor.StopOperation("");
                    IMap pMap = AAState._editor.Map;
                    IActiveView actView = (IActiveView)pMap;
                    actView.PartialRefresh(esriViewDrawPhase.esriViewGeography, null, null);

                    if (MessageBox.Show("Bulk Update complete", "Bulk Update Tool", MessageBoxButtons.OK) == DialogResult.OK)
                    {
                        BulkWorkForm.ActiveForm.Close();
                    }

                }
            }
            catch (Exception exc)
            {

            }

                

                
            }
        


        public void CreateFeature(IFeatureClass featureClass,IFeature sourceF, ESRI.ArcGIS.Geometry.IGeometry shape , string value1, string value2, string value3,string value4, string value5)
        {
            // Ensure the feature class contains points.
            if (featureClass.ShapeType != ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPoint)
            {
                return;
            }
            IFeature feature = featureClass.CreateFeature();
            feature.Shape = shape;

            if (featureClass.AliasName == ConfigUtil.GetConfigValue("ServiceWorksPoint"))
            {
                // Build the feature.
               
                // Update the value on a string field that indicates who installed the feature.
                int contractorFieldIndex = featureClass.FindField("SERVICEWORKSTYPE");
                feature.set_Value(contractorFieldIndex, value1);

                int diaFieldIndex = featureClass.FindField("PIPEDIAMETER");
                feature.set_Value(diaFieldIndex, Convert.ToInt16(value2));

                int matFieldIndex = featureClass.FindField("MATERIAL");
                feature.set_Value(matFieldIndex, value3);


                int ownedbyFieldIndex = featureClass.FindField("OWNEDBY");
                feature.set_Value(ownedbyFieldIndex, value4);

                int JBPFieldIndex = featureClass.FindField("JPNUM");
                feature.set_Value(JBPFieldIndex, value5);

                feature.set_Value(featureClass.FindField("LOCATION"), sourceF.get_Value(sourceF.Fields.FindField(ConfigUtil.GetConfigValue("LOCATION"))));
                feature.set_Value(featureClass.FindField("LOCATIONDESCRIPTION"), ConfigUtil.GetConfigValue("LOCATIONDESCRIPTION"));
                feature.set_Value(featureClass.FindField("FACILITYID"), GenerateID(ConfigUtil.GetConfigValue("GenIDNWM")));

            }
            else if (featureClass.AliasName == ConfigUtil.GetConfigValue("CustomerMeter"))
            {
                int contractorFieldIndex = featureClass.FindField("METERFITTYPE");
                feature.set_Value(contractorFieldIndex, value1);

                int diaFieldIndex = featureClass.FindField("METERTYPE");
                feature.set_Value(diaFieldIndex, value2);

                int matFieldIndex = featureClass.FindField("METERSIZE");
                feature.set_Value(matFieldIndex, value3);


                int ownedbyFieldIndex = featureClass.FindField("CUSTOMERTYPE");
                feature.set_Value(ownedbyFieldIndex, Convert.ToInt16(value4));

                int JBPFieldIndex = featureClass.FindField("JPNUM");
                feature.set_Value(JBPFieldIndex, value5);
                feature.set_Value(featureClass.FindField("LOCATION"), sourceF.get_Value(sourceF.Fields.FindField(ConfigUtil.GetConfigValue("LOCATION"))));
                feature.set_Value(featureClass.FindField("LOCATIONDESCRIPTION"), ConfigUtil.GetConfigValue("LOCATIONDESCRIPTION"));
                feature.set_Value(featureClass.FindField("FACILITYID"), GenerateID(ConfigUtil.GetConfigValue("GenIDNCM")));
            }


            // Commit the new feature to the geodatabase.
            feature.Store();
        }
        public void CreateLineFeature(IFeatureClass featureClass,IFeature sourceF, ESRI.ArcGIS.Geometry.IGeometry shape, string value1, string value2, string value3, string value4, string value5)
        {
            try
            {

                double angle = Convert.ToDouble(ConfigUtil.GetConfigValue("Angle"));
                // Build the feature.
                ESRI.ArcGIS.Geometry.IPoint pOriginPoint = shape as ESRI.ArcGIS.Geometry.IPoint;
                double dbPi = 4 * Math.Atan(1);
                ESRI.ArcGIS.Geometry.IPoint pEndPoint = new ESRI.ArcGIS.Geometry.PointClass();
                //MessageBox.Show(pOriginPoint.X.ToString() + pOriginPoint.Y.ToString());
                pEndPoint.X = pOriginPoint.X - (Math.Cos(((angle) * (dbPi / 180))) * 6);
                pEndPoint.Y = pOriginPoint.Y - (Math.Sin(((angle) * (dbPi / 180))) * 6);


                ESRI.ArcGIS.Geometry.IPointCollection pCollection = new ESRI.ArcGIS.Geometry.PolylineClass();
                pCollection.AddPoint(pOriginPoint);
                pCollection.AddPoint(pEndPoint);

                IFeature feature = featureClass.CreateFeature();
                feature.Shape = pCollection as ESRI.ArcGIS.Geometry.IPolyline;

                // Update the value on a string field that indicates who installed the feature.
                int contractorFieldIndex = featureClass.FindField("DIAMETER");
                feature.set_Value(contractorFieldIndex, Convert.ToInt16(value2));

                int diaFieldIndex = featureClass.FindField("OWNEDBY");
                feature.set_Value(diaFieldIndex, value4);

                int matFieldIndex = featureClass.FindField("SERVICELENGTHBAND");
                feature.set_Value(matFieldIndex, value3);


                int ownedbyFieldIndex = featureClass.FindField("SERVICESIDE");
                feature.set_Value(ownedbyFieldIndex, value1);

                int JBPFieldIndex = featureClass.FindField("JPNUM");
                feature.set_Value(featureClass.FindField("LENGTH"), 6);
                feature.set_Value(JBPFieldIndex, value5);
                feature.set_Value(featureClass.FindField("LOCATION"), sourceF.get_Value(sourceF.Fields.FindField(ConfigUtil.GetConfigValue("LOCATION"))));
                feature.set_Value(featureClass.FindField("LOCATIONDESCRIPTION"), ConfigUtil.GetConfigValue("LOCATIONDESCRIPTION"));
                feature.set_Value(featureClass.FindField("FACILITYID"), GenerateID(ConfigUtil.GetConfigValue("GenIDNWL")));


                // Commit the new feature to the geodatabase.
                feature.Store();

            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message + e.StackTrace);
            }
        }

        private string GenerateID(string valData)
        {
           string sequenceColumnName, sequenceFixedWidth, sequencePostfix = "";
           int sequenceColumnNum;
           int sequenceIntColumnNum;
           int sequencePadding;
           string formatString;
           string[] args = null;
           string output = null;
           List<int> intFldIdxs = new List<int>();

           ITable _gentab = Globals.FindTable(ArcMap.Application, "GenerateId");
            try
            {
                AAState.WriteLine(A4LGSharedFunctions.Localizer.GetString("AttributeAssistantEditorMess_14ar") + "GENERATE_ID");
                if (_gentab != null)
                {
                    sequenceColumnName = "";
                    sequenceColumnNum = -1;

                    sequenceFixedWidth = "";
                    sequencePadding = 0;
                    formatString = "";
                    bool onlyWhenNull = false;

                    // Parse arguments
                    if (valData != null)
                    args = valData.Split('|');
                    switch (args.GetLength(0))
                    {
                        case 1:  // sequenceColumnName only
                            sequenceColumnName = args[0].ToString(); break;
                        case 2:  // sequenceColumnName|sequenceFixedWidth
                            sequenceColumnName = args[0].ToString();
                            sequenceFixedWidth = args[1].ToString();
                            break;
                        case 3:  // sequenceColumnName|sequenceFixedWidth|formatString
                            sequenceColumnName = args[0].ToString();
                            sequenceFixedWidth = args[1].ToString();
                            formatString = args[2].ToString();
                            break;
                        case 4:  // sequenceColumnName|sequenceFixedWidth|formatString
                            sequenceColumnName = args[0].ToString();
                            sequenceFixedWidth = args[1].ToString();
                            formatString = args[2].ToString();
                            onlyWhenNull = args[3].ToString().ToUpper() == "TRUE" ? true : false;

                            break;
                        default: break;
                    }
                    //object val = inObject.get_Value(intFldIdxs[0]);
                    bool proceed = true;
                    //if (onlyWhenNull &&
                    //    (inObject.get_Value(intFldIdxs[0]) != null &&
                    //    inObject.get_Value(intFldIdxs[0]) != DBNull.Value &&
                    //    inObject.get_Value(intFldIdxs[0]).ToString().Trim() != "")
                    //    )
                    //{
                    //    proceed = false;
                    //}
                    if (proceed)
                    {
                        //Check for requested zero padding of sequence number
                        if (sequenceFixedWidth != "")
                            int.TryParse(sequenceFixedWidth.ToString(), out sequencePadding);
                        if (sequencePadding > 25)
                        {

                            AAState.WriteLine(A4LGSharedFunctions.Localizer.GetString("AttributeAssistantEditorChain116"));
                            AAState.WriteLine("                  WARNING: " + sequencePadding + " 0's is what you have");

                        }
                        else if (sequencePadding > 50)
                        {
                            MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("AttributeAssistantEditorChain117"));
                        }
                        qFilter = new QueryFilterClass();
                        qFilter.WhereClause = "SEQNAME = '" + sequenceColumnName + "'";

                        sequenceColumnNum = AAState._gentab.Fields.FindField("SEQCOUNTER");
                        sequenceIntColumnNum = AAState._gentab.Fields.FindField("SEQINTERV");
                        ITransactions pTras = null;

                        long sequenceValue = unversionedEdit(qFilter, sequenceColumnNum, sequenceIntColumnNum, 1, ref pTras);
                        //Debug.WriteLine(sequenceValue.ToString());
                        pTras = null;

                        if (sequenceValue == -1)
                        {
                            AAState.WriteLine(A4LGSharedFunctions.Localizer.GetString("AttributeAssistantEditorError_14a") + "GENERATE_ID: " + A4LGSharedFunctions.Localizer.GetString("AttributeAssistantEditorError_14ao"));
                        }

                        else
                        {
                            
                                if (formatString == null || formatString == "" || formatString.ToLower().IndexOf("[seq]") == -1)
                                {
                                    string setVal = (sequenceValue.ToString("D" + sequencePadding) + sequencePostfix).ToString();

                                    output = (sequenceValue.ToString("D" + sequencePadding) + sequencePostfix).Trim();




                                }
                                else
                                {


                                    int locIdx = formatString.ToUpper().IndexOf("[SEQ]");
                                    if (locIdx >= 0)
                                    {
                                        formatString = formatString.Remove(locIdx, 5);
                                        formatString = formatString.Insert(locIdx, sequenceValue.ToString("D" + sequencePadding));
                                    }
                                    //formatString = formatString.Replace("[seq]", sequenceValue.ToString("D" + sequencePadding));


     
                                    output =  formatString.Trim();
   


                                }

                        }
                    }

                }
                else
                {
                    AAState.WriteLine(A4LGSharedFunctions.Localizer.GetString("AttributeAssistantEditorError_14a") + "GENERATE_ID: " + A4LGSharedFunctions.Localizer.GetString("AttributeAssistantEditorError_14ap"));

                }

            }


            catch (Exception ex)
            {
                AAState.WriteLine(A4LGSharedFunctions.Localizer.GetString("AttributeAssistantEditorError_14a") + "GENERATE_ID: " + ex.Message);
            }
            finally
            {
                if (qFilter != null)
                {
                    Marshal.ReleaseComObject(qFilter);
                    GC.Collect(300);
                    GC.WaitForFullGCComplete();
                    qFilter = null;

                }
                AAState.WriteLine(A4LGSharedFunctions.Localizer.GetString("AttributeAssistantEditorMess_14as") + "GENERATE_ID");
            }
            return output;
        }

        public long unversionedEdit(IQueryFilter qFilterGen, int sequenceColumnNum, int idxSeqField, int curLoop, ref ITransactions transactions)
        {
            long sequenceValue = -1;
            using (ComReleaser comReleaser = new ComReleaser())
            {

                if (AAState._gentabWorkspace.IsInEditOperation)
                {
                    // throw new Exception("Cannot use ITransactions during an edit operation.");
                }
                // Begin a transaction.
                if (transactions == null)
                {
                    transactions = (ITransactions)AAState._gentabWorkspace;
                }

                transactions.StartTransaction();
                try
                {
                    // Use ITable.Update to create an update cursor.
                    ICursor seq_updateCursor = AAState._gentab.Update(qFilterGen, true);
                    comReleaser.ManageLifetime(seq_updateCursor);

                    IRow seq_row = null;
                    seq_row = seq_updateCursor.NextRow();
                    int sequenceInt = 1;

                    if (seq_row != null)
                    {
                        if (idxSeqField > 0)
                        {
                            object seqInt = seq_row.get_Value(idxSeqField);
                            if (seqInt != null)
                            {
                                if (seqInt != DBNull.Value)
                                    try
                                    {
                                        sequenceInt = Convert.ToInt32(seqInt);
                                    }
                                    catch
                                    {

                                    }
                            }
                        }
                        object seqValue = seq_row.get_Value(sequenceColumnNum);

                        if (seqValue == null)
                        {
                            sequenceValue = 0;
                        }
                        else if (seqValue.ToString() == "")
                        {
                            sequenceValue = 0;
                        }
                        else
                            try
                            {
                                sequenceValue = Convert.ToInt64(seqValue);
                            }
                            catch
                            {
                                sequenceValue = 0;
                            }

                        AAState.WriteLine("                  " + sequenceValue + " is the existing value and the interval is " + sequenceInt + ": " + DateTime.Now.ToString("h:mm:ss tt"));

                        sequenceValue = sequenceValue + sequenceInt;

                        seq_row.set_Value(sequenceColumnNum, sequenceValue);
                        AAState.WriteLine("                  " + seq_row.Fields.get_Field(sequenceColumnNum).AliasName + " changed to " + sequenceValue + ": " + DateTime.Now.ToString("h:mm:ss tt"));

                        seq_updateCursor.UpdateRow(seq_row);

                        transactions.CommitTransaction();

                        seq_updateCursor = AAState._gentab.Search(qFilter, true);
                        if (seq_row != null)
                        {
                            seqValue = seq_row.get_Value(sequenceColumnNum);

                            if (seqValue == null)
                            {
                                return sequenceValue;
                            }
                            else if (seqValue.ToString() == "")
                            {
                                return sequenceValue;
                            }
                            else
                                try
                                {
                                    if (sequenceValue == Convert.ToInt64(seqValue))
                                    {
                                        return sequenceValue;
                                    }
                                    else
                                    {
                                        if (curLoop > 30)
                                        {
                                            MessageBox.Show("A unique ID could not be generated after 30 attempts: " + DateTime.Now.ToString("h:mm:ss tt"));
                                        }
                                        else
                                        {
                                            return unversionedEdit(qFilterGen, sequenceColumnNum, idxSeqField, curLoop + 1, ref transactions);
                                        }
                                    }
                                }
                                catch
                                {
                                    return sequenceValue;
                                }
                        }
                        return sequenceValue;
                    }
                    else
                    {
                        AAState.WriteLine("                  No records found in Generate ID table" + ": " + DateTime.Now.ToString("h:mm:ss tt"));
                        transactions.AbortTransaction();
                        return -1;
                    }
                }
                catch (COMException comExc)
                {
                    AAState.WriteLine("                  Error saving transaction to DB" + ": " + DateTime.Now.ToString("h:mm:ss tt"));

                    // If an error occurs during the inserts and updates, rollback the transaction.
                    transactions.AbortTransaction();
                    return -1;
                }


            }

        }


   

        private void rbFerrule_CheckedChanged_1(object sender, EventArgs e)
        {
            if (rbTappingSaddle.Checked == true)
            {
                rbFerrule.Checked = false;
            }
            txtFerMat.Enabled = (rbFerrule.CheckState == CheckState.Checked);
            txtFerDia.Enabled = (rbFerrule.CheckState == CheckState.Checked);
            txtFerJP.Enabled = (rbFerrule.CheckState == CheckState.Checked);
            txtFerSC.Enabled = (rbFerrule.CheckState == CheckState.Checked);

        }

        private void rbTappingSaddle_CheckedChanged_1(object sender, EventArgs e)
        {
            if (rbFerrule.Checked == true)
            {
                rbTappingSaddle.Checked = false;
            }

            txtTapMat.Enabled = (rbTappingSaddle.CheckState == CheckState.Checked);
            txtTSDia.Enabled = (rbTappingSaddle.CheckState == CheckState.Checked);
            txtTSJP.Enabled = (rbTappingSaddle.CheckState == CheckState.Checked);
            txtTSSC.Enabled = (rbTappingSaddle.CheckState == CheckState.Checked);
        }

        private void rbPipeShort_CheckedChanged_1(object sender, EventArgs e)
        {
            if (rbPipeLong.Checked == true)
            {
                rbPipeShort.Checked = false;
            }
            txtSPSLengthBand.Enabled = (rbPipeShort.CheckState == CheckState.Checked);
            txtSPSDia.Enabled = (rbPipeShort.CheckState == CheckState.Checked);
            txtSPSJP.Enabled = (rbPipeShort.CheckState == CheckState.Checked);
            txtSPSSS.Enabled = (rbPipeShort.CheckState == CheckState.Checked);
               
        }

        private void rbPipeLong_CheckedChanged_1(object sender, EventArgs e)
        {
            if (rbPipeShort.Checked == true)
            {
                rbPipeLong.Checked = false;
            }
            txtSPLLengthBand.Enabled = (rbPipeLong.CheckState == CheckState.Checked);
            txtSPLDia.Enabled = (rbPipeLong.CheckState == CheckState.Checked);
            txtSPLJP.Enabled = (rbPipeLong.CheckState == CheckState.Checked);
            txtSPLSS.Enabled = (rbPipeLong.CheckState == CheckState.Checked);
               
        }

        private void rbBoundaryBox_CheckedChanged(object sender, EventArgs e)
        {
            txtBBMat.Enabled = (rbBoundaryBox.CheckState == CheckState.Checked);
            txtBBDia.Enabled = (rbBoundaryBox.CheckState == CheckState.Checked);
            txtBBJP.Enabled = (rbBoundaryBox.CheckState == CheckState.Checked);
            txtBBSC.Enabled = (rbBoundaryBox.CheckState == CheckState.Checked);
        }

        private void rbExisiting_CheckedChanged(object sender, EventArgs e)
        {

            txtESMat.Enabled = (rbExisiting.CheckState == CheckState.Checked);
            txtESDia.Enabled = (rbExisiting.CheckState == CheckState.Checked);
            txtESJP.Enabled = (rbExisiting.CheckState == CheckState.Checked);
            txtESSC.Enabled = (rbExisiting.CheckState == CheckState.Checked);

        }

        private void rbCustomerMeter_CheckedChanged(object sender, EventArgs e)
        {
            if (rbCustomerMeterNonDom.Checked == true)
            {
                rbCustomerMeterDom.Checked = false;
            }
            txtCMeterFitType.Enabled = (rbCustomerMeterDom.CheckState == CheckState.Checked);
            txtCMMeterType.Enabled = (rbCustomerMeterDom.CheckState == CheckState.Checked);
            txtCMJP.Enabled = (rbCustomerMeterDom.CheckState == CheckState.Checked);
            txtCMMeterSize.Enabled = (rbCustomerMeterDom.CheckState == CheckState.Checked);


        }

        private void rbCustomerMeterNonDom_CheckedChanged(object sender, EventArgs e)
        {
            if (rbCustomerMeterDom.Checked == true)
            {
                rbCustomerMeterNonDom.Checked = false;
            }
            txtCMNonDomMeterFitType.Enabled = (rbCustomerMeterNonDom.CheckState == CheckState.Checked);
            txtCMNonDomMeterType.Enabled = (rbCustomerMeterNonDom.CheckState == CheckState.Checked);
            txtCMNonDomMeterSize.Enabled = (rbCustomerMeterNonDom.CheckState == CheckState.Checked);
            txtCMNonDomJobPLan.Enabled   = (rbCustomerMeterNonDom.CheckState == CheckState.Checked);

        }
  
        private void GetConfigSettings()
        {

            AAState._defaultsTableName = ConfigUtil.GetConfigValue("AttributeAssistant_TableName", AAState._defaultsTableName);
            AAState._sequenceTableName = ConfigUtil.GetConfigValue("AttributeAssistant_GenerateId_TableName", AAState._sequenceTableName);
            AAState._CheckEnvelope = ConfigUtil.GetConfigValue("AttributeAssistant_CheckEnvelope ", AAState._CheckEnvelope);



        }

     



  

    






    }
}
