﻿using System.IO;
using System.Text;
using System.Windows;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Validation;

namespace OpenXmlValidatorSampleUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            VersionComBox.ItemsSource = Enum.GetNames<FileFormatVersions>();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var filePath = PptxFilePathTextBox.Text;
                if (string.IsNullOrEmpty(filePath))
                {
                    MessageBox.Show("请输入所要校验的Pptx文件路径");
                    return;
                }

                filePath = filePath.Replace("\"", "");

                if (!File.Exists(filePath))
                {
                    MessageBox.Show("请输入正确的Pptx文件路径");
                    return;
                }


                using var presentationDocument = PresentationDocument.Open(filePath, true);

                var selectVersion = VersionComBox.SelectionBoxItem as string;
                var fileFormatVersions = string.IsNullOrEmpty(selectVersion) ? FileFormatVersions.Office2007 : Enum.Parse<FileFormatVersions>(selectVersion);
                fileFormatVersions = fileFormatVersions is FileFormatVersions.None ? FileFormatVersions.Office2007 : fileFormatVersions;
                var openXmlValidator = new OpenXmlValidator(fileFormatVersions);
                var stringBuilder = new StringBuilder();
                OutputTextBox.Text = string.Empty;
                stringBuilder.AppendLine("开始进行校验....");
                foreach (var validationErrorInfo in openXmlValidator.Validate(presentationDocument))
                {
                    var errorMessage = $"Node：【{validationErrorInfo.Node}】，XmlPath: Uri=【{validationErrorInfo.Path?.PartUri}】，Path=【{validationErrorInfo.Path?.XPath}】，Description:【{validationErrorInfo.Description}】";
                    stringBuilder.AppendLine(errorMessage);
                }
                stringBuilder.AppendLine("执行校验完成....");

                OutputTextBox.Text = stringBuilder.ToString();
            }
            catch (Exception exception)
            {
                OutputTextBox.Text = $"校验完成，存在异常信息：{exception.Message}";
            }


        }
    }
}