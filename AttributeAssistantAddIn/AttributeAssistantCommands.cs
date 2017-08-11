/*
 | Version 10.4
 | Copyright 2016 Esri
 |
 | Licensed under the Apache License, Version 2.0 (the "License");
 | you may not use this file except in compliance with the License.
 | You may obtain a copy of the License at
 |
 |    http://www.apache.org/licenses/LICENSE-2.0
 |
 | Unless required by applicable law or agreed to in writing, software
 | distributed under the License is distributed on an "AS IS" BASIS,
 | WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 | See the License for the specific language governing permissions and
 | limitations under the License.
 */

using System.Diagnostics;
using System;
using System.Drawing;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.JTX;
using ESRI.ArcGIS.JTX.Utilities;
using ESRI.ArcGIS.JTXUI;
using System.Reflection;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Editor;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using A4LGSharedFunctions;


namespace ArcGIS4LocalGovernment
{
    public class MonitorAA : ESRI.ArcGIS.Desktop.AddIns.Button
    {

        public MonitorAA()
        {
            ConfigUtil.type = "aa";

        }

        protected override void OnClick()
        {
            ConfigUtil.type = "aa";

        }
        protected override void Dispose(bool value)
        {

            base.Dispose(value);

        }
        protected override void OnUpdate()
        {
            if (AAState._Suspend == true)
            { }

        }


    }
    public class AttributeAssistantLoadLastValue : ESRI.ArcGIS.Desktop.AddIns.Button
    {

        public AttributeAssistantLoadLastValue()
        {
            ConfigUtil.type = "aa";

        }

        protected override void OnClick()
        {
            ConfigUtil.type = "aa";
            AAState.promptLastValueProperrtySetOneForm();

        }
        protected override void Dispose(bool value)
        {

            base.Dispose(value);

        }
        protected override void OnUpdate()
        {

        }


    }
    public class AttributeAssistantToggleCommand : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        private IEditor m_Editor;

        public AttributeAssistantToggleCommand()
        {
            ConfigUtil.type = "aa";
            m_Editor = Globals.getEditor(ArcMap.Application);

        }

        protected override void OnClick()
        {
            ConfigUtil.type = "aa";
            try
            {


                if (AAState.PerformUpdates)
                {
                    AAState.WriteLine(A4LGSharedFunctions.Localizer.GetString("AttributeAssistantMess_1a"));
                    AAState.PerformUpdates = false;

                    AAState.unInitEditing();
                }
                else
                {
                    AAState.WriteLine(A4LGSharedFunctions.Localizer.GetString("AttributeAssistantMess_1b"));
                    AAState.PerformUpdates = true;
                    AAState.initEditing();
                }
            }
            catch
            { }

        }
        protected override void Dispose(bool value)
        {
            m_Editor = null;
            base.Dispose(value);

        }
        protected override void OnUpdate()
        {
            try
            {
                AAState.setIcon();
            }
            catch
            { }
        }

    }
    public class AttributeAssistantSuspendCommand : ESRI.ArcGIS.Desktop.AddIns.Button
    {

        public AttributeAssistantSuspendCommand()
        {
            ConfigUtil.type = "aa";

        }

        protected override void OnClick()
        {
            ConfigUtil.type = "aa";

            if (AAState._Suspend)
            {
                AAState.WriteLine(A4LGSharedFunctions.Localizer.GetString("AttributeAssistantMess_1c"));
                AAState._Suspend = false;

            }
            else
            {

                AAState.WriteLine(A4LGSharedFunctions.Localizer.GetString("AttributeAssistantMess_1a"));
                AAState._Suspend = true;

            }


        }
        protected override void Dispose(bool value)
        {

            base.Dispose(value);

        }
        protected override void OnUpdate()
        {

        }


    }
    public class AttributeAssistantSuspendOnCommand : ESRI.ArcGIS.Desktop.AddIns.Button
    {

        public AttributeAssistantSuspendOnCommand()
        {

            ConfigUtil.type = "aa";
        }

        protected override void OnClick()
        {
            ConfigUtil.type = "aa";

            AAState._Suspend = false;
            AAState.WriteLine(A4LGSharedFunctions.Localizer.GetString("AttributeAssistantMess_1d"));


        }
        protected override void Dispose(bool value)
        {
            base.Dispose(value);

        }
        protected override void OnUpdate()
        {

        }


    }
    public class AttributeAssistantSuspendOffCommand : ESRI.ArcGIS.Desktop.AddIns.Button
    {

        public AttributeAssistantSuspendOffCommand()
        {

            ConfigUtil.type = "aa";
        }

        protected override void OnClick()
        {
            ConfigUtil.type = "aa";

            AAState._Suspend = true;
            AAState.WriteLine(A4LGSharedFunctions.Localizer.GetString("AttributeAssistantMess_1e"));


        }
        protected override void Dispose(bool value)
        {
            base.Dispose(value);

        }
        protected override void OnUpdate()
        {

        }


    }
    public class RunChangeRulesCommand : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        private IEditor _editor;

        public RunChangeRulesCommand()
        {

            ConfigUtil.type = "aa";
            UID editorUID = new UIDClass();
            editorUID.Value = "esriEditor.editor";
            _editor = ArcMap.Application.FindExtensionByCLSID(editorUID) as IEditor;

            if (_editor != null)
            {

                return;
            }




        }

        protected override void OnClick()
        {
            ConfigUtil.type = "aa";
            if (AAState.PerformUpdates == false)
            {
                MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("AttributeAssistantMess_1f"));
                return;

            }
            if (_editor.EditState == esriEditState.esriStateNotEditing)
            {
                MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("AttributeAssistantMess_1g"));
                return;

            }
            RunChangeRules();
        }

        protected override void OnUpdate()
        {

            if (AAState.PerformUpdates == false)
            {
                Enabled = false;

            }
            else
            {
                Enabled = true;
            }

        }

        public void RunChangeRules()
        {
            ICursor cursor = null; IFeatureCursor fCursor = null;
            bool ran = false;

            try
            {
                //Get list of editable layers
                IEditor editor = _editor;
                IEditLayers eLayers = (IEditLayers)editor;
                long lastOID = -1;
                string lastLay = "";

                if (_editor.EditState != esriEditState.esriStateEditing)
                    return;

                IMap map = editor.Map;
                IActiveView activeView = map as IActiveView;


                IStandaloneTable stTable;

                ITableSelection tableSel;

                IStandaloneTableCollection stTableColl = (IStandaloneTableCollection)map;

                long rowCount = stTableColl.StandaloneTableCount;
                long rowSelCount = 0;
                for (int i = 0; i < stTableColl.StandaloneTableCount; i++)
                {

                    stTable = stTableColl.get_StandaloneTable(i);
                    tableSel = (ITableSelection)stTable;
                    if (tableSel.SelectionSet != null)
                    {
                        rowSelCount = rowSelCount + tableSel.SelectionSet.Count;
                    }


                }
                long featCount = map.SelectionCount;
                int totalCount = (Convert.ToInt32(rowSelCount) + Convert.ToInt32(featCount));


                if (totalCount >= 1)
                {

                    if (MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("AttributeAssistantAsk_2a") + totalCount + A4LGSharedFunctions.Localizer.GetString("AttributeAssistantAsk_2b"),
                        A4LGSharedFunctions.Localizer.GetString("Confirm"), System.Windows.Forms.MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                    {


                        ESRI.ArcGIS.esriSystem.ITrackCancel trackCancel = new ESRI.ArcGIS.Display.CancelTrackerClass();

                        ESRI.ArcGIS.Framework.IProgressDialogFactory progressDialogFactory = new ESRI.ArcGIS.Framework.ProgressDialogFactoryClass();

                        // Set the properties of the Step Progressor
                        System.Int32 int32_hWnd = ArcMap.Application.hWnd;
                        ESRI.ArcGIS.esriSystem.IStepProgressor stepProgressor = progressDialogFactory.Create(trackCancel, int32_hWnd);
                        stepProgressor.MinRange = 1;
                        stepProgressor.MaxRange = totalCount;

                        stepProgressor.StepValue = 1;
                        stepProgressor.Message = A4LGSharedFunctions.Localizer.GetString("AttributeAssistantMess_2a");

                        // Create the ProgressDialog. This automatically displays the dialog
                        ESRI.ArcGIS.Framework.IProgressDialog2 progressDialog2 = (ESRI.ArcGIS.Framework.IProgressDialog2)stepProgressor; // Explict Cast

                        // Set the properties of the ProgressDialog
                        progressDialog2.CancelEnabled = true;
                        progressDialog2.Description = A4LGSharedFunctions.Localizer.GetString("AttributeAssistantDesc_2a") + totalCount.ToString() + ".";
                        progressDialog2.Title = A4LGSharedFunctions.Localizer.GetString("AttributeAssistantTitle_2a");
                        progressDialog2.Animation = ESRI.ArcGIS.Framework.esriProgressAnimationTypes.esriProgressGlobe;

                        // Step. Do your big process here.
                        System.Boolean boolean_Continue = false;
                        boolean_Continue = true;
                        System.Int32 progressVal = 0;

                        ESRI.ArcGIS.esriSystem.IStatusBar statusBar = ArcMap.Application.StatusBar;






                        ran = true;


                        editor.StartOperation();


                        //Get list of feature layers
                        UID geoFeatureLayerID = new UIDClass();
                        geoFeatureLayerID.Value = "{E156D7E5-22AF-11D3-9F99-00C04F6BC78E}";
                        IEnumLayer enumLayer = map.get_Layers(((ESRI.ArcGIS.esriSystem.UID)geoFeatureLayerID), true);
                        IFeatureLayer fLayer;
                        IFeatureSelection fSel;
                        ILayer layer;
                        // Step through each geofeature layer in the map
                        enumLayer.Reset();
                        // Create an edit operation enabling undo/redo
                        try
                        {


                            //   AAState.StopChangeMonitor();
                            while ((layer = enumLayer.Next()) != null)
                            {
                                // Verify that this is a valid, visible layer and that this layer is editable
                                fLayer = (IFeatureLayer)layer;

                                bool bIsEditableFabricLayer = (fLayer is ICadastralFabricSubLayer2);
                                if (bIsEditableFabricLayer)
                                {
                                    IDataset pDS = (IDataset)fLayer.FeatureClass;
                                    IWorkspace pFabWS = pDS.Workspace;
                                    bIsEditableFabricLayer = (bIsEditableFabricLayer && pFabWS.Equals(editor.EditWorkspace));
                                }
                                if (fLayer.Valid && (eLayers.IsEditable(fLayer) || bIsEditableFabricLayer))//fLayer.Visible &&
                                {
                                    // Verify that this layer has selected features  
                                    IFeatureClass fc = fLayer.FeatureClass;
                                    fSel = (IFeatureSelection)fLayer;

                                    if ((fc != null) && (fSel.SelectionSet.Count > 0))
                                    {


                                        fSel.SelectionSet.Search(null, false, out cursor);
                                        fCursor = cursor as IFeatureCursor;
                                        IFeature feat;
                                        while ((feat = (IFeature)fCursor.NextFeature()) != null)
                                        {
                                            progressVal++;
                                            progressDialog2.Description = A4LGSharedFunctions.Localizer.GetString("AttributeAssistantProc_2a") + progressVal + A4LGSharedFunctions.Localizer.GetString("Of") + totalCount.ToString() + ".";

                                            stepProgressor.Step();

                                            statusBar.set_Message(0, progressVal.ToString());

                                            lastLay = fLayer.Name;


                                            lastOID = feat.OID;
                                            IObject pObj = feat as IObject;





                                            AAState.WriteLine(A4LGSharedFunctions.Localizer.GetString("AttributeAssistantMess_2c"));
                                            AAState._editEvents.OnChangeFeature -= AAState.FeatureChange;


                                            try
                                            {
                                                AAState.FeatureChange(pObj);
                                                feat.Store();
                                            }
                                            catch
                                            {
                                            }


                                            AAState._editEvents.OnChangeFeature += AAState.FeatureChange;
                                            AAState.WriteLine(A4LGSharedFunctions.Localizer.GetString("AttributeAssistantMess_2b"));

                                            //Check if the cancel button was pressed. If so, stop process
                                            boolean_Continue = trackCancel.Continue();
                                            if (!boolean_Continue)
                                            {
                                                break;
                                            }


                                        }
                                        if (feat != null)
                                            Marshal.ReleaseComObject(feat);



                                    }

                                }
                                else
                                {
                                    if (fLayer.Valid)
                                    {
                                        // Verify that this layer has selected features  
                                        IFeatureClass fc = fLayer.FeatureClass;
                                        fSel = (IFeatureSelection)fLayer;
                                        if (fSel.SelectionSet.Count > 0)
                                        {
                                            progressVal = progressVal + fSel.SelectionSet.Count;
                                            stepProgressor.OffsetPosition(progressVal);

                                            stepProgressor.Step();

                                        }
                                    }

                                }
                            }
                            activeView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
                        }

                        catch (Exception ex)
                        {
                            editor.AbortOperation();
                            ran = false;
                            MessageBox.Show("RunChangeRule\n" + ex.Message + " \n" + lastLay + ": " + lastOID, ex.Source);
                            return;
                        }
                        finally
                        {
                            //  AAState.StartChangeMonitor();
                            try
                            {
                                // Stop the edit operation 
                                editor.StopOperation(A4LGSharedFunctions.Localizer.GetString("AttributeAssistantDone_2a"));
                            }
                            catch (Exception ex)
                            { }

                        }







                        editor.StartOperation();
                        try
                        {
                            AAState.StopChangeMonitor();
                            for (int i = 0; i < stTableColl.StandaloneTableCount; i++)
                            {

                                stTable = stTableColl.get_StandaloneTable(i);
                                tableSel = (ITableSelection)stTable;
                                if (tableSel.SelectionSet != null)
                                {

                                    if (tableSel.SelectionSet.Count > 0)
                                    {

                                        tableSel.SelectionSet.Search(null, false, out  cursor);
                                        IRow pRow;
                                        while ((pRow = (IRow)cursor.NextRow()) != null)
                                        {
                                            progressVal++;
                                            progressDialog2.Description = A4LGSharedFunctions.Localizer.GetString("AttributeAssistantProc_2a") + progressVal + A4LGSharedFunctions.Localizer.GetString("Of") + totalCount.ToString() + ".";
                                            stepProgressor.Step();


                                            statusBar.set_Message(0, progressVal.ToString());

                                            lastOID = pRow.OID;
                                            lastLay = stTable.Name;

                                            IObject pObj = pRow as IObject;
                                            AAState.WriteLine(A4LGSharedFunctions.Localizer.GetString("AttributeAssistantMess_2c"));
                                            AAState._editEvents.OnChangeFeature -= AAState.FeatureChange;


                                            try
                                            {
                                                AAState.FeatureChange(pObj);
                                                pRow.Store();
                                            }
                                            catch
                                            {
                                            }


                                            AAState._editEvents.OnChangeFeature += AAState.FeatureChange;
                                            AAState.WriteLine(A4LGSharedFunctions.Localizer.GetString("AttributeAssistantMess_2b"));

                                            //Check if the cancel button was pressed. If so, stop process
                                            boolean_Continue = trackCancel.Continue();
                                            if (!boolean_Continue)
                                            {
                                                break;
                                            }
                                        }
                                        if (pRow != null)
                                            Marshal.ReleaseComObject(pRow);

                                        pRow = null;


                                    }
                                }
                            }

                        }
                        catch (Exception ex)
                        {
                            editor.AbortOperation();
                            MessageBox.Show("RunChangeRules\n" + ex.Message + " \n" + lastLay + ": " + lastOID, ex.Source);
                            ran = false;

                            return;
                        }
                        finally
                        {
                            AAState.StartChangeMonitor();
                            try
                            {
                                // Stop the edit operation 
                                editor.StopOperation(A4LGSharedFunctions.Localizer.GetString("AttributeAssistantDone_2a"));
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("AttributeAssistantEditorWarn_14a") + " " + ex.Message);

                            }

                        }
                        // Done
                        trackCancel = null;
                        stepProgressor = null;
                        progressDialog2.HideDialog();
                        progressDialog2 = null;
                    }

                }
                else
                {
                    MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("AttributeAssistantError_2a"));

                }

            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + " \n" + "RunChangeRules", ex.Source);
                ran = false;

                return;
            }
            finally
            {
                if (ran)
                    //  MessageBox.Show("Process has completed successfully");

                    if (cursor != null)
                        Marshal.ReleaseComObject(cursor);
                if (fCursor != null)
                    Marshal.ReleaseComObject(fCursor);

            }
        }


    }
    public class RunChangeGeoRulesCommand : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        private IEditor _editor;

        public RunChangeGeoRulesCommand()
        {

            ConfigUtil.type = "aa";
            UID editorUID = new UIDClass();
            editorUID.Value = "esriEditor.editor";
            _editor = ArcMap.Application.FindExtensionByCLSID(editorUID) as IEditor;

            if (_editor != null)
            {

                return;
            }




        }

        protected override void OnClick()
        {
            ConfigUtil.type = "aa";
            if (AAState.PerformUpdates == false)
            {
                MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("AttributeAssistantMess_1f"));
                return;

            }
            if (_editor.EditState == esriEditState.esriStateNotEditing)
            {
                MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("AttributeAssistantMess_1g"));
                return;

            }
            RunChangeGeoRules();
        }

        protected override void OnUpdate()
        {

            if (AAState.PerformUpdates == false)
            {
                Enabled = false;

            }
            else
            {
                Enabled = true;
            }

        }

        public void RunChangeGeoRules()
        {
            ICursor cursor = null; IFeatureCursor fCursor = null;
            bool ran = false;

            try
            {
                //Get list of editable layers
                IEditor editor = _editor;
                IEditLayers eLayers = (IEditLayers)editor;
                long lastOID = -1;
                string lastLay = "";

                if (_editor.EditState != esriEditState.esriStateEditing)
                    return;

                IMap map = editor.Map;
                IActiveView activeView = map as IActiveView;


                long featCount = map.SelectionCount;
                int totalCount = Convert.ToInt32(featCount);


                if (totalCount >= 1)
                {

                    if (MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("AttributeAssistantAsk_2a") + totalCount + A4LGSharedFunctions.Localizer.GetString("AttributeAssistantAsk_2b"),
                        A4LGSharedFunctions.Localizer.GetString("Confirm"), System.Windows.Forms.MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                    {



                        ESRI.ArcGIS.esriSystem.ITrackCancel trackCancel = new ESRI.ArcGIS.Display.CancelTrackerClass();

                        ESRI.ArcGIS.Framework.IProgressDialogFactory progressDialogFactory = new ESRI.ArcGIS.Framework.ProgressDialogFactoryClass();

                        // Set the properties of the Step Progressor
                        System.Int32 int32_hWnd = ArcMap.Application.hWnd;
                        ESRI.ArcGIS.esriSystem.IStepProgressor stepProgressor = progressDialogFactory.Create(trackCancel, int32_hWnd);
                        stepProgressor.MinRange = 1;
                        stepProgressor.MaxRange = totalCount;

                        stepProgressor.StepValue = 1;
                        stepProgressor.Message = A4LGSharedFunctions.Localizer.GetString("AttributeAssistantMess_2a");

                        // Create the ProgressDialog. This automatically displays the dialog
                        ESRI.ArcGIS.Framework.IProgressDialog2 progressDialog2 = (ESRI.ArcGIS.Framework.IProgressDialog2)stepProgressor; // Explict Cast

                        // Set the properties of the ProgressDialog
                        progressDialog2.CancelEnabled = true;
                        progressDialog2.Description = A4LGSharedFunctions.Localizer.GetString("AttributeAssistantDesc_2a") + totalCount.ToString() + ".";
                        progressDialog2.Title = A4LGSharedFunctions.Localizer.GetString("AttributeAssistantTitle_2a");
                        progressDialog2.Animation = ESRI.ArcGIS.Framework.esriProgressAnimationTypes.esriProgressGlobe;

                        // Step. Do your big process here.
                        System.Boolean boolean_Continue = false;
                        boolean_Continue = true;
                        System.Int32 progressVal = 0;

                        ESRI.ArcGIS.esriSystem.IStatusBar statusBar = ArcMap.Application.StatusBar;



                        ran = true;


                        editor.StartOperation();

                        //bool test = false;

                        //Get list of feature layers
                        UID geoFeatureLayerID = new UIDClass();
                        geoFeatureLayerID.Value = "{E156D7E5-22AF-11D3-9F99-00C04F6BC78E}";
                        IEnumLayer enumLayer = map.get_Layers(((ESRI.ArcGIS.esriSystem.UID)geoFeatureLayerID), true);
                        IFeatureLayer fLayer;
                        IFeatureSelection fSel;
                        ILayer layer;
                        // Step through each geofeature layer in the map
                        enumLayer.Reset();
                        // Create an edit operation enabling undo/redo
                        try
                        {
                            //   AAState.StopChangeMonitor();
                            while ((layer = enumLayer.Next()) != null)
                            {
                                // Verify that this is a valid, visible layer and that this layer is editable
                                fLayer = (IFeatureLayer)layer;
                                bool bIsEditableFabricLayer = (fLayer is ICadastralFabricSubLayer2);
                                if (bIsEditableFabricLayer)
                                {
                                    IDataset pDS = (IDataset)fLayer.FeatureClass;
                                    IWorkspace pFabWS = pDS.Workspace;
                                    bIsEditableFabricLayer = (bIsEditableFabricLayer && pFabWS.Equals(editor.EditWorkspace));
                                }
                                if (fLayer.Valid && (eLayers.IsEditable(fLayer) || bIsEditableFabricLayer))//fLayer.Visible &&
                                {
                                    // Verify that this layer has selected features  
                                    IFeatureClass fc = fLayer.FeatureClass;
                                    fSel = (IFeatureSelection)fLayer;

                                    if ((fc != null) && (fSel.SelectionSet.Count > 0))
                                    {



                                        fSel.SelectionSet.Search(null, false, out cursor);
                                        fCursor = cursor as IFeatureCursor;
                                        IFeature feat;
                                        while ((feat = (IFeature)fCursor.NextFeature()) != null)
                                        {

                                            progressVal++;
                                            progressDialog2.Description = A4LGSharedFunctions.Localizer.GetString("AttributeAssistantProc_2a") + progressVal + A4LGSharedFunctions.Localizer.GetString("Of") + totalCount.ToString() + ".";

                                            stepProgressor.Step();

                                            statusBar.set_Message(0, progressVal.ToString());



                                            lastLay = fLayer.Name;


                                            lastOID = feat.OID;
                                            IObject pObj = feat as IObject;


                                            AAState.WriteLine(A4LGSharedFunctions.Localizer.GetString("AttributeAssistantMess_2c"));
                                            AAState._editEvents.OnChangeFeature -= AAState.FeatureChange;


                                            try
                                            {
                                                AAState.FeatureGeoChange(pObj);
                                                feat.Store();
                                            }
                                            catch
                                            {
                                            }


                                            AAState._editEvents.OnChangeFeature += AAState.FeatureChange;
                                            AAState.WriteLine(A4LGSharedFunctions.Localizer.GetString("AttributeAssistantMess_2b"));

                                            //Check if the cancel button was pressed. If so, stop process
                                            boolean_Continue = trackCancel.Continue();
                                            if (!boolean_Continue)
                                            {
                                                break;
                                            }


                                        }
                                        if (feat != null)
                                            Marshal.ReleaseComObject(feat);



                                    }

                                }

                            }

                        }
                        catch (Exception ex)
                        {
                            editor.AbortOperation();
                            ran = false;
                            MessageBox.Show("RunChangeRule\n" + ex.Message + " \n" + lastLay + ": " + lastOID, ex.Source);
                            return;
                        }
                        finally
                        {
                            //  AAState.StartChangeMonitor();
                            try
                            {
                                // Stop the edit operation 
                                editor.StopOperation("Run Change Geo Rules - Features");
                            }
                            catch (Exception ex)
                            { }

                        }


                        // Done
                        trackCancel = null;
                        stepProgressor = null;
                        progressDialog2.HideDialog();
                        progressDialog2 = null;




                    }

                }
                else
                {
                    MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("AttributeAssistantError_2a"));

                }
                activeView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);

            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + " \n" + "RunChangeGeoRules", ex.Source);
                ran = false;

                return;
            }
            finally
            {
                if (ran)
                    //MessageBox.Show("Process has completed successfully");

                    if (cursor != null)
                        Marshal.ReleaseComObject(cursor);
                if (fCursor != null)
                    Marshal.ReleaseComObject(fCursor);

            }
        }


    }
    public class RunManualRulesCommand : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        private IEditor _editor;

        public RunManualRulesCommand()
        {

            ConfigUtil.type = "aa";
            UID editorUID = new UIDClass();
            editorUID.Value = "esriEditor.editor";
            _editor = ArcMap.Application.FindExtensionByCLSID(editorUID) as IEditor;

            if (_editor != null)
            {

                return;
            }




        }

        protected override void OnClick()
        {
            ConfigUtil.type = "aa";
            if (AAState.PerformUpdates == false)
            {
                MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("AttributeAssistantMess_1f"));
                return;

            }
            if (_editor.EditState == esriEditState.esriStateNotEditing)
            {
                MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("AttributeAssistantMess_1g"));
                return;

            }
            RunManualRules();
        }

        protected override void OnUpdate()
        {

            if (AAState.PerformUpdates == false)
            {
                Enabled = false;

            }
            else
            {
                Enabled = true;
            }

        }
        public void RunManualRules()
        {
            ICursor cursor = null; IFeatureCursor fCursor = null;
            bool ran = false;

            try
            {
                //Get list of editable layers
                IEditor editor = _editor;
                IEditLayers eLayers = (IEditLayers)editor;
                long lastOID = -1;
                string lastLay = "";

                if (_editor.EditState != esriEditState.esriStateEditing)
                    return;

                IMap map = editor.Map;
                IActiveView activeView = map as IActiveView;


                IStandaloneTable stTable;

                ITableSelection tableSel;

                IStandaloneTableCollection stTableColl = (IStandaloneTableCollection)map;

                long rowCount = stTableColl.StandaloneTableCount;
                long rowSelCount = 0;
                for (int i = 0; i < stTableColl.StandaloneTableCount; i++)
                {

                    stTable = stTableColl.get_StandaloneTable(i);
                    tableSel = (ITableSelection)stTable;
                    if (tableSel.SelectionSet != null)
                    {
                        rowSelCount = rowSelCount + tableSel.SelectionSet.Count;
                    }

                }
                long featCount = map.SelectionCount;
                int totalCount = (Convert.ToInt32(rowSelCount) + Convert.ToInt32(featCount));


                if (totalCount >= 1)
                {


                    if (MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("AttributeAssistantAsk_3a") + totalCount + A4LGSharedFunctions.Localizer.GetString("AttributeAssistantAsk_2b"),
                        A4LGSharedFunctions.Localizer.GetString("Confirm"), System.Windows.Forms.MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                    {


                        ESRI.ArcGIS.esriSystem.ITrackCancel trackCancel = new ESRI.ArcGIS.Display.CancelTrackerClass();

                        ESRI.ArcGIS.Framework.IProgressDialogFactory progressDialogFactory = new ESRI.ArcGIS.Framework.ProgressDialogFactoryClass();

                        // Set the properties of the Step Progressor
                        System.Int32 int32_hWnd = ArcMap.Application.hWnd;
                        ESRI.ArcGIS.esriSystem.IStepProgressor stepProgressor = progressDialogFactory.Create(trackCancel, int32_hWnd);
                        stepProgressor.MinRange = 1;
                        stepProgressor.MaxRange = totalCount;

                        stepProgressor.StepValue = 1;
                        stepProgressor.Message = "Running Manual Rules";

                        // Create the ProgressDialog. This automatically displays the dialog
                        ESRI.ArcGIS.Framework.IProgressDialog2 progressDialog2 = (ESRI.ArcGIS.Framework.IProgressDialog2)stepProgressor; // Explict Cast

                        // Set the properties of the ProgressDialog
                        progressDialog2.CancelEnabled = true;
                        progressDialog2.Description = A4LGSharedFunctions.Localizer.GetString("AttributeAssistantDesc_2a") + totalCount.ToString() + ".";
                        progressDialog2.Title = A4LGSharedFunctions.Localizer.GetString("AttributeAssistantTitle_2a");
                        progressDialog2.Animation = ESRI.ArcGIS.Framework.esriProgressAnimationTypes.esriProgressGlobe;

                        // Step. Do your big process here.
                        System.Boolean boolean_Continue = false;
                        boolean_Continue = true;
                        System.Int32 progressVal = 0;

                        ESRI.ArcGIS.esriSystem.IStatusBar statusBar = ArcMap.Application.StatusBar;

                        editor.StartOperation();


                        ran = true;


                        //bool test = false;

                        //Get list of feature layers
                        UID geoFeatureLayerID = new UIDClass();
                        geoFeatureLayerID.Value = "{E156D7E5-22AF-11D3-9F99-00C04F6BC78E}";
                        IEnumLayer enumLayer = map.get_Layers(((ESRI.ArcGIS.esriSystem.UID)geoFeatureLayerID), true);
                        IFeatureLayer fLayer;
                        IFeatureSelection fSel;
                        ILayer layer;
                        // Step through each geofeature layer in the map
                        enumLayer.Reset();
                        // Create an edit operation enabling undo/redo
                        try
                        {
                            while ((layer = enumLayer.Next()) != null)
                            {
                                // Verify that this is a valid, visible layer and that this layer is editable
                                fLayer = (IFeatureLayer)layer;
                                bool bIsEditableFabricLayer = (fLayer is ICadastralFabricSubLayer2);
                                if (bIsEditableFabricLayer)
                                {
                                    IDataset pDS = (IDataset)fLayer.FeatureClass;
                                    IWorkspace pFabWS = pDS.Workspace;
                                    bIsEditableFabricLayer = (bIsEditableFabricLayer && pFabWS.Equals(editor.EditWorkspace));
                                }
                                if (fLayer.Valid && (eLayers.IsEditable(fLayer) || bIsEditableFabricLayer))//fLayer.Visible &&
                                {
                                    // Verify that this layer has selected features  
                                    IFeatureClass fc = fLayer.FeatureClass;
                                    fSel = (IFeatureSelection)fLayer;

                                    if ((fc != null) && (fSel.SelectionSet.Count > 0))
                                    {

                                        // test = true;

                                        fSel.SelectionSet.Search(null, false, out cursor);
                                        fCursor = cursor as IFeatureCursor;
                                        IFeature feat;
                                        while ((feat = (IFeature)fCursor.NextFeature()) != null)
                                        {

                                            progressVal++;
                                            progressDialog2.Description = A4LGSharedFunctions.Localizer.GetString("AttributeAssistantProc_2a") + progressVal + A4LGSharedFunctions.Localizer.GetString("Of") + totalCount.ToString() + ".";

                                            stepProgressor.Step();

                                            statusBar.set_Message(0, progressVal.ToString());

                                            lastLay = fLayer.Name;


                                            lastOID = feat.OID;
                                            IObject pObj = feat as IObject;
                                            AAState.WriteLine(A4LGSharedFunctions.Localizer.GetString("AttributeAssistantMess_4a"));
                                            AAState._editEvents.OnChangeFeature -= AAState.FeatureChange;


                                            try
                                            {
                                                AAState.FeatureManual(pObj);
                                                feat.Store();
                                            }
                                            catch
                                            { }


                                            AAState._editEvents.OnChangeFeature += AAState.FeatureChange;
                                            AAState.WriteLine(A4LGSharedFunctions.Localizer.GetString("AttributeAssistantMess_4b"));

                                            //Check if the cancel button was pressed. If so, stop process
                                            boolean_Continue = trackCancel.Continue();
                                            if (!boolean_Continue)
                                            {
                                                break;
                                            }
                                        }
                                        if (feat != null)
                                            Marshal.ReleaseComObject(feat);



                                    }

                                }
                                else
                                {
                                    if (fLayer.Valid)
                                    {
                                        // Verify that this layer has selected features  
                                        IFeatureClass fc = fLayer.FeatureClass;
                                        fSel = (IFeatureSelection)fLayer;
                                        if (fSel.SelectionSet.Count > 0)
                                        {
                                            progressVal = progressVal + fSel.SelectionSet.Count;
                                            stepProgressor.OffsetPosition(progressVal);

                                            stepProgressor.Step();

                                        }
                                    }

                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            editor.AbortOperation();
                            ran = false;
                            MessageBox.Show("RunManualRules\n" + ex.Message + " \n" + lastLay + ": " + lastOID, ex.Source);
                            return;
                        }
                        finally
                        {


                        }








                        try
                        {
                            for (int i = 0; i < stTableColl.StandaloneTableCount; i++)
                            {

                                stTable = stTableColl.get_StandaloneTable(i);
                                tableSel = (ITableSelection)stTable;
                                if (tableSel.SelectionSet != null)
                                {

                                    if (tableSel.SelectionSet.Count > 0)
                                    {
                                        tableSel.SelectionSet.Search(null, false, out  cursor);
                                        IRow pRow;
                                        while ((pRow = (IRow)cursor.NextRow()) != null)
                                        {

                                            progressVal++;
                                            progressDialog2.Description = A4LGSharedFunctions.Localizer.GetString("AttributeAssistantProc_2a") + progressVal + A4LGSharedFunctions.Localizer.GetString("Of") + totalCount.ToString() + ".";
                                            stepProgressor.Step();


                                            statusBar.set_Message(0, progressVal.ToString());

                                            lastOID = pRow.OID;
                                            lastLay = stTable.Name;

                                            IObject pObj = pRow as IObject;
                                            AAState.WriteLine(A4LGSharedFunctions.Localizer.GetString("AttributeAssistantMess_4a"));
                                            AAState._editEvents.OnChangeFeature -= AAState.FeatureChange;


                                            try
                                            {
                                                AAState.FeatureManual(pObj);
                                                pRow.Store();
                                            }
                                            catch
                                            { }


                                            AAState._editEvents.OnChangeFeature += AAState.FeatureChange;
                                            AAState.WriteLine(A4LGSharedFunctions.Localizer.GetString("AttributeAssistantMess_4b"));

                                            //Check if the cancel button was pressed. If so, stop process
                                            boolean_Continue = trackCancel.Continue();
                                            if (!boolean_Continue)
                                            {
                                                break;
                                            }
                                        }
                                        if (pRow != null)
                                            Marshal.ReleaseComObject(pRow);


                                    }
                                }
                            }
                            activeView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
                        }
                        catch (Exception ex)
                        {
                            editor.AbortOperation();
                            MessageBox.Show("RunManualRules\n" + ex.Message + " \n" + lastLay + ": " + lastOID, ex.Source);
                            ran = false;

                            return;
                        }
                        finally
                        {


                        }
                        try
                        {
                            editor.StopOperation(A4LGSharedFunctions.Localizer.GetString("AttributeAssistantDone_4a"));

                        }
                        catch
                        {


                        }
                        // Done
                        trackCancel = null;
                        stepProgressor = null;
                        progressDialog2.HideDialog();
                        progressDialog2 = null;
                    }

                }
                else
                {
                    MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("AttributeAssistantError_2a"));

                }

            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + " \n" + "RunManualRules", ex.Source);
                ran = false;

                return;
            }
            finally
            {
                if (ran)
                    //MessageBox.Show("Process has completed successfully");

                    if (cursor != null)
                        Marshal.ReleaseComObject(cursor);
                if (fCursor != null)
                    Marshal.ReleaseComObject(fCursor);

            }
        }

    }
    public class RunCreateRulesCommand : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        private IEditor _editor;

        public RunCreateRulesCommand()
        {

            ConfigUtil.type = "aa";
            UID editorUID = new UIDClass();
            editorUID.Value = "esriEditor.editor";
            _editor = ArcMap.Application.FindExtensionByCLSID(editorUID) as IEditor;

            if (_editor != null)
            {

                return;
            }




        }

        protected override void OnClick()
        {
            ConfigUtil.type = "aa";
            if (AAState.PerformUpdates == false)
            {
                MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("AttributeAssistantMess_1f"));
                return;

            }
            if (_editor.EditState == esriEditState.esriStateNotEditing)
            {
                MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("AttributeAssistantMess_1g"));
                return;

            }
            RunCreateRules();
        }

        protected override void OnUpdate()
        {

            if (AAState.PerformUpdates == false)
            {
                Enabled = false;

            }
            else
            {
                Enabled = true;
            }

        }

        public void RunCreateRules()
        {
            ICursor cursor = null; IFeatureCursor fCursor = null;
            bool ran = false;

            try
            {
                //Get list of editable layers
                IEditor editor = _editor;
                IEditLayers eLayers = (IEditLayers)editor;
                long lastOID = -1;
                string lastLay = "";

                if (_editor.EditState != esriEditState.esriStateEditing)
                    return;

                IMap map = editor.Map;
                IActiveView activeView = map as IActiveView;


                IStandaloneTable stTable;

                ITableSelection tableSel;

                IStandaloneTableCollection stTableColl = (IStandaloneTableCollection)map;

                long rowCount = stTableColl.StandaloneTableCount;
                long rowSelCount = 0;
                for (int i = 0; i < stTableColl.StandaloneTableCount; i++)
                {

                    stTable = stTableColl.get_StandaloneTable(i);
                    tableSel = (ITableSelection)stTable;
                    if (tableSel.SelectionSet != null)
                    {
                        rowSelCount = rowSelCount + tableSel.SelectionSet.Count;
                    }


                }
                long featCount = map.SelectionCount;
                int totalCount = (Convert.ToInt32(rowSelCount) + Convert.ToInt32(featCount));


                if (totalCount >= 1)
                {

                    if (MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("AttributeAssistantAsk_4a") + totalCount + A4LGSharedFunctions.Localizer.GetString("AttributeAssistantAsk_2b"),
                        A4LGSharedFunctions.Localizer.GetString("Confirm"), System.Windows.Forms.MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                    {



                        ESRI.ArcGIS.esriSystem.ITrackCancel trackCancel = new ESRI.ArcGIS.Display.CancelTrackerClass();

                        ESRI.ArcGIS.Framework.IProgressDialogFactory progressDialogFactory = new ESRI.ArcGIS.Framework.ProgressDialogFactoryClass();

                        // Set the properties of the Step Progressor
                        System.Int32 int32_hWnd = ArcMap.Application.hWnd;
                        ESRI.ArcGIS.esriSystem.IStepProgressor stepProgressor = progressDialogFactory.Create(trackCancel, int32_hWnd);
                        stepProgressor.MinRange = 1;
                        stepProgressor.MaxRange = totalCount;

                        stepProgressor.StepValue = 1;
                        stepProgressor.Message = A4LGSharedFunctions.Localizer.GetString("AttributeAssistantMess_2a");

                        // Create the ProgressDialog. This automatically displays the dialog
                        ESRI.ArcGIS.Framework.IProgressDialog2 progressDialog2 = (ESRI.ArcGIS.Framework.IProgressDialog2)stepProgressor; // Explict Cast

                        // Set the properties of the ProgressDialog
                        progressDialog2.CancelEnabled = true;
                        progressDialog2.Description = A4LGSharedFunctions.Localizer.GetString("AttributeAssistantDesc_2a") + totalCount.ToString() + ".";
                        progressDialog2.Title = A4LGSharedFunctions.Localizer.GetString("AttributeAssistantTitle_2a");
                        progressDialog2.Animation = ESRI.ArcGIS.Framework.esriProgressAnimationTypes.esriProgressGlobe;

                        // Step. Do your big process here.
                        System.Boolean boolean_Continue = false;
                        boolean_Continue = true;
                        System.Int32 progressVal = 0;

                        ESRI.ArcGIS.esriSystem.IStatusBar statusBar = ArcMap.Application.StatusBar;

                        ran = true;
                        editor.StartOperation();

                        //bool test = false;

                        //Get list of feature layers
                        UID geoFeatureLayerID = new UIDClass();
                        geoFeatureLayerID.Value = "{E156D7E5-22AF-11D3-9F99-00C04F6BC78E}";
                        IEnumLayer enumLayer = map.get_Layers(((ESRI.ArcGIS.esriSystem.UID)geoFeatureLayerID), true);
                        IFeatureLayer fLayer;
                        IFeatureSelection fSel;
                        ILayer layer;
                        // Step through each geofeature layer in the map
                        enumLayer.Reset();
                        // Create an edit operation enabling undo/redo
                        try
                        {
                            //   AAState.StopCreateMonitor();
                            while ((layer = enumLayer.Next()) != null)
                            {
                                // Verify that this is a valid, visible layer and that this layer is editable
                                fLayer = (IFeatureLayer)layer;
                                bool bIsEditableFabricLayer = (fLayer is ICadastralFabricSubLayer2);
                                if (bIsEditableFabricLayer)
                                {
                                    IDataset pDS = (IDataset)fLayer.FeatureClass;
                                    IWorkspace pFabWS = pDS.Workspace;
                                    bIsEditableFabricLayer = (bIsEditableFabricLayer && pFabWS.Equals(editor.EditWorkspace));
                                }
                                if (fLayer.Valid && (eLayers.IsEditable(fLayer) || bIsEditableFabricLayer))//fLayer.Visible &&
                                {
                                    // Verify that this layer has selected features  
                                    IFeatureClass fc = fLayer.FeatureClass;
                                    fSel = (IFeatureSelection)fLayer;

                                    if ((fc != null) && (fSel.SelectionSet.Count > 0))
                                    {


                                        fSel.SelectionSet.Search(null, false, out cursor);
                                        fCursor = cursor as IFeatureCursor;
                                        IFeature feat;
                                        feat = (IFeature)fCursor.NextFeature();
                                        while (feat != null)
                                        {


                                            progressVal++;
                                            progressDialog2.Description = A4LGSharedFunctions.Localizer.GetString("AttributeAssistantProc_2a") + progressVal + A4LGSharedFunctions.Localizer.GetString("Of") + totalCount.ToString() + ".";

                                            stepProgressor.Step();

                                            statusBar.set_Message(0, progressVal.ToString());


                                            lastOID = feat.OID;
                                            lastLay = fLayer.Name;
                                            IObject pObj = feat as IObject;


                                            AAState.WriteLine(A4LGSharedFunctions.Localizer.GetString("AttributeAssistantMess_5a"));
                                            AAState._editEvents.OnCreateFeature -= AAState.FeatureCreate;
                                            AAState._editEvents.OnChangeFeature -= AAState.FeatureChange;


                                            try
                                            {
                                                //AAState.FeatureCreate(pObj);
                                                Debug.WriteLine("Feature with " + feat.OID);
                                                Debug.WriteLine(progressVal.ToString() + " : Count is " + fSel.SelectionSet.Count);
                                                feat.Store();
                                            }
                                            catch
                                            {
                                            }


                                            AAState._editEvents.OnCreateFeature += AAState.FeatureCreate;
                                            AAState._editEvents.OnChangeFeature += AAState.FeatureChange;
                                            AAState.WriteLine(A4LGSharedFunctions.Localizer.GetString("AttributeAssistantMess_5b"));

                                            //Check if the cancel button was pressed. If so, stop process
                                            boolean_Continue = trackCancel.Continue();
                                            if (!boolean_Continue)
                                            {
                                                break;
                                            }
                                            feat = (IFeature)fCursor.NextFeature();
                                        }
                                        if (feat != null)
                                            Marshal.ReleaseComObject(feat);



                                    }

                                }
                                else
                                {
                                    if (fLayer.Valid)
                                    {
                                        // Verify that this layer has selected features  
                                        IFeatureClass fc = fLayer.FeatureClass;
                                        fSel = (IFeatureSelection)fLayer;
                                        if (fSel.SelectionSet.Count > 0)
                                        {
                                            progressVal = progressVal + fSel.SelectionSet.Count;
                                            stepProgressor.OffsetPosition(progressVal);

                                            stepProgressor.Step();

                                        }
                                    }

                                }
                            }

                            activeView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
                        }
                        catch (Exception ex)
                        {
                            editor.AbortOperation();
                            ran = false;
                            MessageBox.Show("RunCreateRule\n" + ex.Message + " \n" + lastLay + ": " + lastOID, ex.Source);
                            return;
                        }
                        finally
                        {
                            //  AAState.StartCreateMonitor();
                            try
                            {
                                // Stop the edit operation 
                                editor.StopOperation(A4LGSharedFunctions.Localizer.GetString("AttributeAssistantDone_5a"));
                            }
                            catch (Exception ex)
                            { }

                        }







                        editor.StartOperation();
                        try
                        {
                            AAState.StopChangeMonitor();
                            for (int i = 0; i < stTableColl.StandaloneTableCount; i++)
                            {

                                stTable = stTableColl.get_StandaloneTable(i);
                                tableSel = (ITableSelection)stTable;
                                if (tableSel.SelectionSet != null)
                                {

                                    if (tableSel.SelectionSet.Count > 0)
                                    {
                                        tableSel.SelectionSet.Search(null, false, out  cursor);
                                        IRow pRow;
                                        while ((pRow = (IRow)cursor.NextRow()) != null)
                                        {

                                            progressVal++;
                                            progressDialog2.Description = A4LGSharedFunctions.Localizer.GetString("AttributeAssistantProc_2a") + progressVal + A4LGSharedFunctions.Localizer.GetString("Of") + totalCount.ToString() + ".";
                                            stepProgressor.Step();


                                            statusBar.set_Message(0, progressVal.ToString());


                                            lastOID = pRow.OID;
                                            lastLay = stTable.Name;
                                            IObject pObj = pRow as IObject;


                                            AAState.WriteLine(A4LGSharedFunctions.Localizer.GetString("AttributeAssistantMess_5a"));
                                            AAState._editEvents.OnCreateFeature -= AAState.FeatureCreate;


                                            try
                                            {
                                                AAState.FeatureCreate(pObj);
                                                pRow.Store();
                                            }
                                            catch
                                            {
                                            }


                                            AAState._editEvents.OnCreateFeature += AAState.FeatureCreate;
                                            AAState.WriteLine(A4LGSharedFunctions.Localizer.GetString("AttributeAssistantMess_5b"));

                                            //Check if the cancel button was pressed. If so, stop process
                                            boolean_Continue = trackCancel.Continue();
                                            if (!boolean_Continue)
                                            {
                                                break;
                                            }
                                        }
                                        if (pRow != null)
                                            Marshal.ReleaseComObject(pRow);


                                    }
                                }
                            }

                        }
                        catch (Exception ex)
                        {
                            editor.AbortOperation();
                            MessageBox.Show("RunCreateRules\n" + ex.Message + " \n" + lastLay + ": " + lastOID, ex.Source);
                            ran = false;

                            return;
                        }
                        finally
                        {
                            AAState.StartChangeMonitor();
                            try
                            {
                                // Stop the edit operation 
                                editor.StopOperation(A4LGSharedFunctions.Localizer.GetString("AttributeAssistantDone_5a"));
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("AttributeAssistantEditorWarn_14a") + " " + ex.Message);

                            }

                        }

                        // Done
                        trackCancel = null;
                        stepProgressor = null;
                        progressDialog2.HideDialog();
                        progressDialog2 = null;
                    }

                }
                else
                {
                    MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("AttributeAssistantError_2a"));

                }

            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + " \n" + "RunCreateRules", ex.Source);
                ran = false;

                return;
            }
            finally
            {
                if (ran)
                    //  MessageBox.Show("Process has completed successfully");

                    if (cursor != null)
                        Marshal.ReleaseComObject(cursor);
                if (fCursor != null)
                    Marshal.ReleaseComObject(fCursor);

            }
        }


    }
    public class ShowConfigForm : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        //internal static ESRI.ArcGIS.Framework.IDockableWindow s_dockWindow;
        ConfigForm m_ConfigForm;
        public ShowConfigForm()
        {
            ConfigUtil.type = "aa";
        }

        protected override void OnClick()
        {
            //DockableWindow pDockWin = getDockableWindow() as DockableWindow;
            //if (pDockWin == null)
            //    return;
            //pDockWin.Show(!pDockWin.IsVisible());
            //string ConfigPath = ConfigUtil.generateUserCachePath();

            //string[] ConfigFiles = ConfigUtil.GetConfigFiles();
            ConfigUtil.type = "aa";
            m_ConfigForm = new ConfigForm("aa");
            m_ConfigForm.ShowDialog();

        }

        protected override void OnUpdate()
        {
            Enabled = (ArcMap.Application != null);

        }
        //private ESRI.ArcGIS.Framework.IDockableWindow getDockableWindow()
        //{

        //    // Only get/create the dockable window if they ask for it
        //    if (s_dockWindow == null)
        //    {
        //        UID dockWinID = new UID();
        //        dockWinID.Value = "A4WaterUtilities_ConfigDetails";
        //        s_dockWindow = ArcMap.DockableWindowManager.GetDockableWindow(dockWinID);
        //        //s_extension.UpdateSelCountDockWin()
        //    }

        //    return s_dockWindow;
        //}
    }
    public class BulkWorkScopeCommand : ESRI.ArcGIS.Desktop.AddIns.Button
    {

        private IEditor _editor;
        BulkWorkForm m_bulkworkform;
        public BulkWorkScopeCommand()
        {
            ConfigUtil.type = "aa";
            UID editorUID = new UIDClass();
            editorUID.Value = "esriEditor.editor";
            _editor = ArcMap.Application.FindExtensionByCLSID(editorUID) as IEditor;

            if (_editor != null)
            {

                return;
            }

        }
        protected override void OnClick()
        {

            ConfigUtil.type = "aa";

            if (AAState.PerformUpdates == false)
            {
                MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("AttributeAssistantMess_1f"));
                return;

            }
            if (_editor.EditState == esriEditState.esriStateNotEditing)
            {
                MessageBox.Show(A4LGSharedFunctions.Localizer.GetString("AttributeAssistantMess_1g"));
                return;

            }
            IFeatureLayer sourceLayer;
            bool boolLayerOrFC = true;
            string classItem;
            sourceLayer = Globals.FindLayer(AAState._editor.Map, "wMain", ref boolLayerOrFC) as IFeatureLayer;

            if (AAState._editor.SelectionCount <= 0)
            {
                MessageBox.Show("Please select Designed WPRN features to run this tool");
            }
            else
            {
                IEnumFeature selection = AAState._editor.EditSelection;
                IObject obj = selection.Next();
                while (obj != null)
                {
                    if (obj.Class.AliasName == ConfigUtil.GetConfigValue("DESIGNEDWPRN"))
                    {
                        ShowForm();
                        return;
                    }

                    obj = selection.Next();

                }
                MessageBox.Show("Please select Designed WPRN  features to run this tool");
            }



        }
        public void ShowForm()
        {
            m_bulkworkform = new BulkWorkForm();
            m_bulkworkform.ShowDialog();
        }
        protected override void OnUpdate()
        {
            Enabled = (ArcMap.Application != null);

        }

        //

    }
    public class CreateJob : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        private IJTXDatabaseManager2 m_wmxDbMgr = null;
        private IJTXDatabase3 m_previousWmxDb = null;
        private string m_jobTypeAsString = ConfigUtil.GetConfigValue("jobTypeAsString");
        private string m_jobOwner = ConfigUtil.GetConfigValue("jobOwner");
        private string m_assigneeType = ConfigUtil.GetConfigValue("assigneeType");
        private string m_assignee = ConfigUtil.GetConfigValue("assignee");
        private ILayer m_aoiLayer = null;
        private DateTime m_startDate = DateTime.Now;
        private DateTime m_dueDate;
        private string m_priority = string.Empty;
        private int m_parentJobId = -1;
        private string m_dataWorkspaceId = string.Empty;
        private string m_parentVersion = string.Empty;
        private bool m_executeNewJob = false;
        private Dictionary<string, IJTXDatabase3> m_wmxDbInfo = new Dictionary<string, IJTXDatabase3>();
        private const string C_OPT_ASSIGN_TO_GROUP = "ASSIGN_TO_GROUP";
        private const string C_OPT_ASSIGN_TO_USER = "ASSIGN_TO_USER";
        private const string C_OPT_UNASSIGNED = "UNASSIGNED";
        private const string C_OPT_VAL_UNASSIGNED = "[Unassigned]";
        private const string C_OPT_VAL_NOT_SET = "[Not set]";
        private const string C_OPT_VAL_EXECUTE = "EXECUTE";
        private const string C_OPT_VAL_NO_EXECUTE = "NO_EXECUTE";
        private const string C_PARAM_JOB_TYPE = "in_string_jobType";
        private const string C_PARAM_JOB_OWNER = "in_string_owner";
        private const string C_PARAM_ASSIGNEE_TYPE = "in_string_assigneeType";
        private const string C_PARAM_ASSIGNEE = "in_string_assignee";
        private const string C_PARAM_AOI = "in_layer_aoi";
        private const string C_PARAM_START_DATE = "in_date_startDate";
        private const string C_PARAM_DUE_DATE = "in_date_dueDate";
        private const string C_PARAM_PRIORITY = "in_string_priority";
        private const string C_PARAM_PARENTJOBID = "in_long_parentJobId";
        private const string C_PARAM_DATAWORKSPACE = "in_string_dataWorkspace";
        private const string C_PARAM_PARENTVERSION = "in_string_parentVersion";
        private const string C_PARAM_EXECUTE_NEW_JOB = "in_bool_executeNewJob";
        private const string C_PARAM_NEWJOBID = "out_long_jobId";
        private const string C_PARAM_WMX_DATABASE_ALIAS = "in_string_wmxDatabaseAlias";

        private const string C_ENV_VAR_DEFAULT_WMX_DB = "DEFAULT_JTX_DB";
        private string m_wmxDbAlias = string.Empty;
        public CreateJob()
        {

        }
        protected override void OnClick()
        {
            Execute();
            ArcMap.Application.CurrentTool = null;
        }
        protected override void OnUpdate()
        {
            Enabled = ArcMap.Application != null;
        }
        public void Execute()
        {
            IJTXJob4 job = null;
            String server = ConfigUtil.GetConfigValue("server");//"VM-IE-IWWNM1";
            String Instance = ConfigUtil.GetConfigValue("Instance");//"VM-IE-IWWNM1\\sqlexpress";
            String Database = ConfigUtil.GetConfigValue("Database");//"WNMDesigned";
            String dbClient = ConfigUtil.GetConfigValue("dbClient");
            String uname = ConfigUtil.GetConfigValue("uname");
            String password = ConfigUtil.GetConfigValue("password");
            IQueryFilter pQF = null;
            IFeatureCursor pFC = null;
            IFeatureClass pFeatureClass = null;
            string sProjectID = null;

            // Do some common error-checking
            //base.Execute(paramValues, trackCancel, envMgr, msgs);

            //////////////////////////////////////////////////////////////////////
            // TODO: Update the job-creation logic.
            //
            // In subsequent builds of Workflow Manager (post-10.0), there may be
            // a new API that creates jobs and handles much of the logic included
            // in this function (and this class at large).  If so, this GP tool
            // should be revised to make use of this simplified interface.
            //
            // Anyone using this tool as a reference, particularly with regards
            // to creating Workflow Manager jobs, should keep this in mind.
            //////////////////////////////////////////////////////////////////////

            // Try to create the job, as requested
            try
            {
                IWorkspace pws = WorkgroupArcSdeWorkspaceFromPropertySet(server, Instance, Database, uname, password, dbClient);
                IJTXJobManager2 jobManager = this.WmxDatabase.JobManager as IJTXJobManager2;
                IJTXConfiguration configMgr = this.WmxDatabase.ConfigurationManager as IJTXConfiguration;
                IJTXJobType4 jobTypeObj = configMgr.GetJobType(m_jobTypeAsString) as IJTXJobType4;
                IFeatureWorkspace pwswkFac = pws as IFeatureWorkspace;
                pFeatureClass = pwswkFac.OpenFeatureClass(ConfigUtil.GetConfigValue("ProjectsFC"));
                pQF = new QueryFilter();
                pQF.WhereClause = "JOBID  IS NULL OR JOBID =''";
                pFC = pFeatureClass.Search(pQF, true);
                IFeature feature = pFC.NextFeature();
                m_aoiLayer = feature as ILayer;

                while (feature != null)
                {

                    // Set up the description object to be used to create this job
                    IJTXJobDescription jobDescription = new JTXJobDescriptionClass();
                    jobDescription.JobTypeName = m_jobTypeAsString;
                    jobDescription.AOI = feature.Shape as IPolygon4;

                    // Set up the ownership & assignment of the job
                    jobDescription.OwnedBy = m_jobOwner;
                    if (m_assigneeType.Equals(C_OPT_ASSIGN_TO_GROUP))
                    {
                        jobDescription.AssignedType = jtxAssignmentType.jtxAssignmentTypeGroup;
                        //jobDescription.AssignedTo = m_assignee;
                        jobDescription.AssignedTo = feature.get_Value(feature.Fields.FindField(m_assignee)).ToString();
                    }
                    else if (m_assigneeType.Equals(C_OPT_ASSIGN_TO_USER))
                    {
                        jobDescription.AssignedType = jtxAssignmentType.jtxAssignmentTypeUser;
                        jobDescription.AssignedTo = m_assignee;
                    }
                    else if (m_assigneeType.Equals(C_OPT_UNASSIGNED))
                    {
                        jobDescription.AssignedType = jtxAssignmentType.jtxAssignmentTypeUnassigned;
                        jobDescription.AssignedTo = string.Empty;
                    }
                    else
                    {
                        // Do nothing; let the job type defaults take over
                        //msgs.AddMessage("Using job type defaults for job assignment");
                        jobDescription.AssignedType = jobTypeObj.DefaultAssignedType;
                        jobDescription.AssignedTo = jobTypeObj.DefaultAssignedTo;
                    }

                    // Start date
                    if (m_startDate != null)
                    {
                        string tempStr = m_startDate.ToString();

                        // Workflow Manager stores times as UTC times; input times must
                        // therefore be pre-converted
                        DateTime tempDate = DateTime.Parse(tempStr);
                        jobDescription.StartDate = TimeZone.CurrentTimeZone.ToUniversalTime(tempDate);
                    }
                    else
                    {
                        //msgs.AddMessage("Using job type defaults for start date");
                        jobDescription.StartDate = jobTypeObj.DefaultStartDate;
                    }

                    // Due date
                    if (m_dueDate != null)
                    {
                        string tempStr = m_dueDate.ToString();

                        // Workflow Manager stores times as UTC times; input times must
                        // therefore be pre-converted
                        DateTime tempDate = DateTime.Parse(tempStr);
                        jobDescription.DueDate = TimeZone.CurrentTimeZone.ToUniversalTime(tempDate);
                    }
                    else
                    {
                        //msgs.AddMessage("Using job type defaults for due date");
                        jobDescription.DueDate = jobTypeObj.DefaultDueDate;
                    }

                    // Priority
                    if (!m_priority.Equals(string.Empty))
                    {
                        IJTXPriority priority = configMgr.GetPriority(m_priority);
                        jobDescription.Priority = priority;
                    }
                    else
                    {
                        //msgs.AddMessage("Using job type defaults for priority");
                        jobDescription.Priority = jobTypeObj.DefaultPriority;
                    }

                    // Parent job
                    if (m_parentJobId > 0)
                    {
                        jobDescription.ParentJobId = m_parentJobId;
                    }

                    // Data workspace
                    if (m_dataWorkspaceId.Equals(C_OPT_VAL_NOT_SET))
                    {
                        jobDescription.DataWorkspaceID = string.Empty;
                    }
                    else if (!m_dataWorkspaceId.Equals(string.Empty))
                    {
                        jobDescription.DataWorkspaceID = m_dataWorkspaceId;
                    }
                    else
                    {
                        //msgs.AddMessage("Using job type defaults for data workspace");
                        if (jobTypeObj.DefaultDataWorkspace != null)
                        {
                            jobDescription.DataWorkspaceID = jobTypeObj.DefaultDataWorkspace.DatabaseID;
                        }
                    }

                    // Parent version
                    if (m_parentVersion.Equals(C_OPT_VAL_NOT_SET))
                    {
                        jobDescription.ParentVersionName = string.Empty;
                    }
                    else if (!m_parentVersion.Equals(string.Empty))
                    {
                        jobDescription.ParentVersionName = m_parentVersion;
                    }
                    else
                    {
                        //msgs.AddMessage("Using job type defaults for parent version");
                        jobDescription.ParentVersionName = jobTypeObj.DefaultParentVersionName;
                    }

                    // Auto-execution
                    jobDescription.Suffix = "_P_" + feature.get_Value(feature.Fields.FindField("WONUM")).ToString();
                    jobDescription.AutoExecuteOnCreate = m_executeNewJob;

                    // Create the new job
                    int expectedNumJobs = 1;
                    bool checkAoi = true;
                    IJTXJobSet jobSet = null;
                    IJTXExecuteInfo execInfo;
                    try
                    {
                        jobSet = jobManager.CreateJobsFromDescription(jobDescription, expectedNumJobs, checkAoi, out execInfo);
                    }
                    catch (System.Runtime.InteropServices.COMException comEx)
                    {
                        throw new Exception("");
                    }

                    if ((execInfo != null && execInfo.ThrewError) ||
                        jobSet == null ||
                        jobSet.Count != expectedNumJobs)
                    {
                        if (execInfo != null && !string.IsNullOrEmpty(execInfo.ErrorDescription))
                        {
                            throw new Exception("");
                        }
                        else
                        {
                            throw new Exception("");
                        }
                    }

                    // If it gets all the way down here without errors, set the output ID with the
                    // ID of the job that was created.
                    job = jobSet.Next() as IJTXJob4;
                    feature.set_Value(feature.Fields.FindField("JOBID"), job.ID.ToString());
                    feature.Store();
                    //MessageBox.Show(job.ID.ToString());

                    //MessageBox.Show(String.Format("Job ID of the new JOB is :{0}", job.ID.ToString()));
                    feature = pFC.NextFeature();
                }


                MessageBox.Show("Preliminary Proposal created for the Project");
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.StackTrace);
                try
                {

                    if (job != null)
                    {
                        this.WmxDatabase.JobManager.DeleteJob(job.ID, true);
                    }
                }
                catch
                {
                    // Catch anything else that possibly happens
                }
            }
            finally
            {
                if (pQF != null)
                {
                    Marshal.ReleaseComObject(pQF);
                    pQF = null;
                }
                if (pFC != null)
                {
                    Marshal.ReleaseComObject(pFC);
                    pFC = null;
                }
            }
        }


        public IJTXDatabase3 WmxDatabase
        {
            get
            {
                if (this.IsWorkflowManagerDatabaseSet())
                {
                    IJTXDatabase3 retVal = m_wmxDbInfo[m_wmxDbAlias];

                    if (retVal != m_previousWmxDb)
                    {
                        // If the database in use has changed, invalidate and reinitialize the
                        // configuration cache.  This is used internally by various Workflow
                        // Manager items; if it is not initialized, some of them will (right
                        // or wrong) throw errors.
                        ESRI.ArcGIS.JTXUI.ConfigurationCache.InvalidateCache();
                        ESRI.ArcGIS.JTXUI.ConfigurationCache.InitializeCache(m_wmxDbInfo[m_wmxDbAlias]);
                        m_previousWmxDb = retVal;
                    }

                    return retVal;
                }
                else
                {
                    // Invalidate the configuration cache if there's a problem finding
                    // the database
                    ESRI.ArcGIS.JTXUI.ConfigurationCache.InvalidateCache();

                    return null;
                }
            }
        }
        protected bool IsWorkflowManagerDatabaseSet()
        {
            bool isDatabaseSet = false;

            try
            {
                // Get a handle to the database manager, if need be
                if (m_wmxDbMgr == null)
                {
                    m_wmxDbMgr = new JTXDatabaseManagerClass();
                }

                // Determine the default Workflow Manager database if none has been specified
                if (string.IsNullOrEmpty(m_wmxDbAlias))
                {
                    // WORKAROUND: Query the environment variable directly for the alias
                    // of the default workflow manager DB.  IJTXDatabaseManager.GetActiveDatabase()
                    // is a fairly expensive call, so this provides a way to avoid making
                    // it unnecessarily.
                    m_wmxDbAlias = System.Environment.GetEnvironmentVariable(C_ENV_VAR_DEFAULT_WMX_DB);
                    if (m_wmxDbAlias == null)
                    {
                        m_wmxDbAlias = string.Empty;
                    }
                }

                // If we don't already have a database object for this database, get one
                // and cache it away
                if (!m_wmxDbInfo.ContainsKey(m_wmxDbAlias))
                {
                    IJTXDatabase3 tempDb = m_wmxDbMgr.GetDatabase(m_wmxDbAlias) as IJTXDatabase3;

                    // If this is the first time we've retrieved the information for the
                    // default database, store it away
                    if (!m_wmxDbInfo.ContainsKey(m_wmxDbAlias) && tempDb != null)
                    {
                        m_wmxDbInfo[m_wmxDbAlias] = tempDb;
                        ESRI.ArcGIS.JTXUI.ConfigurationCache.InvalidateCache();
                        ESRI.ArcGIS.JTXUI.ConfigurationCache.InitializeCache(tempDb);
                    }
                }

                // At this point, the alias should be set, and the wmx DB connection should
                // be stored in the table.  Do one last set of sanity checks before setting
                // the return value.
                if (m_wmxDbInfo.ContainsKey(m_wmxDbAlias) && m_wmxDbInfo[m_wmxDbAlias] != null)
                {
                    isDatabaseSet = true;
                }
            }
            catch (System.Runtime.InteropServices.COMException comEx)
            {
                MessageBox.Show(comEx.StackTrace);
                m_wmxDbInfo.Remove(m_wmxDbAlias);
                m_wmxDbMgr = null;
                m_wmxDbAlias = string.Empty;

                int errorCode;

                // Check for license-related problems
                if (comEx.ErrorCode == (int)ESRI.ArcGIS.JTX.jtxCoreError.E_JTXCORE_ERR_LICENSE_NOT_CHECKED_OUT ||
                    comEx.ErrorCode == (int)ESRI.ArcGIS.JTX.jtxCoreError.E_JTXCORE_ERR_LICENSE_MISSING_PRODUCT ||
                    comEx.ErrorCode == (int)ESRI.ArcGIS.JTX.jtxCoreError.E_JTXCORE_ERR_LICENSE_INCOMPATIBLE_PRODUCT)
                {
                    throw new Exception("");
                }

                unchecked
                {
                    errorCode = (int)0x80040AF4;
                }

                if (comEx.ErrorCode != errorCode)
                {
                    throw comEx;
                }
            }

            return isDatabaseSet;
        }

        public static IWorkspace WorkgroupArcSdeWorkspaceFromPropertySet(String server, String instance, String database, String user, String Password, String dbclient)
        {
            IWorkspace wks = null;
            Type factoryType = null;
            try
            {
                IPropertySet propertySet = new PropertySetClass();
                //propertySet.SetProperty("DB_CONNECTION_PROPERTIES", DBConnProp);

                if (dbclient == "oracle")
                {
                    propertySet.SetProperty("DBCLIENT", dbclient);
                    propertySet.SetProperty("DB_CONNECTION_PROPERTIES", instance);
                    //propertySet.SetProperty("DATABASE", database);
                    propertySet.SetProperty("AUTHENTICATION_MODE", "DBMS");
                    propertySet.SetProperty("USER", user);
                    propertySet.SetProperty("PASSWORD", Password);
                    //propertySet.SetProperty("VERSION", "SDE.DEFAULT");
                    factoryType = Type.GetTypeFromProgID(
                        "esriDataSourcesGDB.SdeWorkspaceFactory");

                }
                else if (dbclient == "SQLServer")
                {
                    propertySet.SetProperty("dbclient", dbclient);
                    propertySet.SetProperty("serverinstance", instance);
                    propertySet.SetProperty("DATABASE", database);
                    propertySet.SetProperty("AUTHENTICATION_MODE", "DBMS");
                    propertySet.SetProperty("USER", user);
                    propertySet.SetProperty("PASSWORD", Password);
                    factoryType = Type.GetTypeFromProgID(
                        "esriDataSourcesGDB.SqlWorkspaceFactory");
                }



                //            factoryType = Type.GetTypeFromProgID(
                //"esriDataSourcesGDB.SqlWorkspaceFactory");
                IWorkspaceFactory workspaceFactory = (IWorkspaceFactory)Activator.CreateInstance
                    (factoryType);
                wks = workspaceFactory.Open(propertySet, 0);
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.StackTrace);
            }

            return wks;
        }
    }


}