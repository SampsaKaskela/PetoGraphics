using System;
using System.IO;
using System.Windows.Threading;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace PetoGraphics
{
    class Config
    {
        public static void Save(string folderName)
        {
            try
            {
                // Create Config folder if it doesn't exist
                string folderpath = AppDomain.CurrentDomain.BaseDirectory + @"Configs\" + folderName;
                if (!Directory.Exists(folderpath))
                {
                    Directory.CreateDirectory(folderpath);
                    Windows.Main.targetFolder.Items.Add(Path.GetFileName(AppDomain.CurrentDomain.BaseDirectory + @"Configs\" + folderName));
                }
                // Create config file if it doesn't exist
                string configfile = folderpath + @"\" + folderName + ".json";
                FileStream fs = null;
                if (!File.Exists(configfile))
                {
                    using (fs = File.Create(configfile)) { }
                }
                else
                {
                    File.Delete(configfile);
                    using (fs = File.Create(configfile)) { }
                }
                if (File.Exists(configfile))
                {
                    Windows.ProgressWindow.progressBar.Value = 0;
                    Windows.ProgressWindow.Show();
                    Windows.ProgressWindow.infoText.Content = "Copying Images";
                    for (int i = 0; i < GraphicController.rootList.Count; i++)
                    {
                        CopyImages(GraphicController.rootList[i], folderName);
                        Windows.ProgressWindow.progressBar.Value = 100 * i / GraphicController.rootList.Count;
                        UpdateUI();
                    }
                    Windows.ProgressWindow.infoText.Content = "Creating config file";
                    UpdateUI();
                    GraphicSerializer serializer = new GraphicSerializer();
                    using (StreamWriter sw = new StreamWriter(configfile))
                    {
                        using (JsonWriter jw = new JsonTextWriter(sw))
                        {
                            jw.Formatting = Formatting.Indented;
                            serializer.Serialize(GraphicController.rootList, folderName).WriteTo(jw);
                        }
                    }
                    Windows.ProgressWindow.Hide();
                    CustomMessageBox.Show("Config file created.");
                }
            }
            catch (Exception e)
            {
                CustomMessageBox.Show("Failed to create config file. " + e.Message, "Error");
            }
        }

        public static void Load(string path)
        {
            ClearGraphics(GraphicController.rootList);
            GraphicController.Selected = null;
            try
            {
                if (File.Exists(path))
                {
                    using (StreamReader sr = new StreamReader(path))
                    {
                        GraphicSerializer serializer = new GraphicSerializer();
                        GraphicController.rootList = serializer.Deserialize(sr.ReadToEnd());
                        HideGraphics(GraphicController.rootList);
                        ControllerHelpers.CheckResize();
                        ControllerHelpers.RepositionControls(GraphicController.rootList);
                        CustomMessageBox.Show("Config file read.");
                    }
                }
                else
                {
                    CustomMessageBox.Show("No config file found.");
                }
            }
            catch { }
        }

        public static int Delete(string path)
        {
            try
            {
                ClearGraphics(GraphicController.rootList);
                GraphicController.Selected = null;
                Directory.Delete(path, true);
                Windows.Main.targetFolder.Items.Remove(Path.GetDirectoryName(path));
                GC.Collect();
                return 0;
            }
            catch (IOException)
            {
                return 1;
            }
            catch (UnauthorizedAccessException)
            {
                return 2;
            }
            catch
            {
                return 3;
            }
        }

        private static void UpdateUI()
        {
            DispatcherFrame frame = new DispatcherFrame();
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Render, new DispatcherOperationCallback(delegate (object parameter)
            {
                frame.Continue = false;
                return null;
            }
            ), null);
            Dispatcher.PushFrame(frame);
        }

        private static void CopyImages(GraphicController controller, string foldername)
        {
            if (controller.Graphic != null)
            {
                if (controller.Graphic.Image != null && controller.Graphic.Image.UriSource != null)
                {
                    if (controller.Graphic.Image.IsSequence)
                    {
                        try
                        {
                            if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + @"Configs\" + foldername + @"\" + Path.GetFileName(Path.GetDirectoryName(controller.Graphic.Image.UriSource))))
                            {
                                Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + @"Configs\" + foldername + @"\" + Path.GetFileName(Path.GetDirectoryName(controller.Graphic.Image.UriSource)));
                            }
                            foreach (string path in controller.Graphic.Image.SequenceFrames)
                            {
                                if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"Configs\" + foldername + @"\" + Path.GetFileName(Path.GetDirectoryName(controller.Graphic.Image.UriSource)) + @"\" + Path.GetFileName(path)))
                                {
                                    File.Copy(path, AppDomain.CurrentDomain.BaseDirectory + @"Configs\" + foldername + @"\" + Path.GetFileName(Path.GetDirectoryName(controller.Graphic.Image.UriSource)) + @"\" + Path.GetFileName(path), true);
                                }
                            }
                        }
                        catch { };
                    }
                    else
                    {
                        try
                        {
                            if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"Configs\" + foldername + @"\" + Path.GetFileName(controller.Graphic.Image.UriSource)))
                            {
                                File.Copy(controller.Graphic.Image.UriSource, AppDomain.CurrentDomain.BaseDirectory + @"Configs\" + foldername + @"\" + Path.GetFileName(controller.Graphic.Image.UriSource), true);
                            }
                        }
                        catch { };
                    }
                }
                if (controller.Graphic is ImageSwitcher)
                {
                    ImageSwitcher imageSwitcher = (ImageSwitcher)controller.Graphic;
                    try
                    {
                        if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + @"Configs\" + foldername + @"\" + Path.GetFileName(imageSwitcher.Folder)))
                        {
                            Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + @"Configs\" + foldername + @"\" + Path.GetFileName(imageSwitcher.Folder));
                        }
                        foreach (string path in imageSwitcher.Images)
                        {
                            if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"Configs\" + foldername + @"\" + Path.GetFileName(imageSwitcher.Folder) + @"\" + Path.GetFileName(path)))
                            {
                                File.Copy(path, AppDomain.CurrentDomain.BaseDirectory + @"Configs\" + foldername + @"\" + Path.GetFileName(imageSwitcher.Folder) + @"\" + Path.GetFileName(path), true);
                            }
                        }
                    }
                    catch { };
                }
            }
            foreach (GraphicController child in controller.Children)
            {
                CopyImages(child, foldername);
            }
        }
        private static void HideGraphics(List<GraphicController> controllers)
        {
            foreach(GraphicController controller in controllers)
            {
                controller.Active = false;
            }
        }

        private static void ClearGraphics(List<GraphicController> list)
        {
            while (list.Count > 0)
            {
                ClearGraphics(list[0].Children);
                list[0].Delete();
            }
        }
    }
}
