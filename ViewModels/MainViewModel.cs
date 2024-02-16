using MessagingToolkit.QRCode.Codec;
using MessagingToolkit.QRCode.Codec.Data;
using Microsoft.Win32;
using QR_Manaeste.Support;
using System;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace QR_Manaeste.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private string _textToEncode;
        private string _decodedText;
        private BitmapImage _generatedQRCode;
        private BitmapImage _decodedQRCode;
        private bool _isCcreateQRSelected;
        private bool _isScanQRSelected;
        private bool _isQRCreated = false;

        public string TextToEncode
        {
            get => _textToEncode;
            set
            {
                _textToEncode = value;
                OnPropertyChanged(nameof(TextToEncode));
            }
        }

        public string DecodedText
        {
            get => _decodedText;
            set
            {
                _decodedText = value;
                OnPropertyChanged(nameof(DecodedText));
            }
        }

        public BitmapImage GeneratedQRCode
        {
            get => _generatedQRCode;
            set
            {
                _generatedQRCode = value;
                OnPropertyChanged(nameof(GeneratedQRCode));
            }
        }
        
        public BitmapImage DecodedQRCode
        {
            get => _decodedQRCode;
            set
            {
                _decodedQRCode = value;
                OnPropertyChanged(nameof(DecodedQRCode));
            }
        }

        public bool IsCreateQRSelected
        {
            get => _isCcreateQRSelected;
            set
            {
                _isCcreateQRSelected = value;
                OnPropertyChanged(nameof(IsCreateQRSelected));
            }
        }

        public bool IsScanQRSelected
        {
            get => _isScanQRSelected;
            set
            {
                _isScanQRSelected = value;
                OnPropertyChanged(nameof(IsScanQRSelected));
            }
        }

        public bool IsQRCreated
        {
            get => _isQRCreated;
            set
            {
                _isQRCreated = value;
                OnPropertyChanged(nameof(IsQRCreated));
            }
        }

        public ICommand GenerateQRCodeCommand { get; private set; }
        public ICommand DecodeQRCodeFromFileCommand { get; private set; }
        public ICommand SaveQRCodeImageCommand { get; private set; }
        public ICommand SwitchToCreateQRCommand { get; private set; }
        public ICommand SwitchToScanQRCommand { get; private set; }
        public ICommand ShareQRCodeCommand { get; private set; }
        public ICommand CopyDecodedTextCommand { get; private set; }

        public MainViewModel()
        {
            TextToEncode = "";
            IsCreateQRSelected = true;
            GenerateQRCodeCommand = new RelayCommand(GenerateQRCode, CanGenerateQRCode);
            DecodeQRCodeFromFileCommand = new RelayCommand(DecodeQRCodeFromFile);
            SaveQRCodeImageCommand = new RelayCommand(SaveQRCodeImage, CanSaveQRCodeImage);
            SwitchToCreateQRCommand = new RelayCommand(SwitchToCreateQR);
            SwitchToScanQRCommand = new RelayCommand(SwitchToScanQR);
            ShareQRCodeCommand = new RelayCommand(ShareQRCode);
            CopyDecodedTextCommand = new RelayCommand(CopyDecodedText, CanCopyDecodedText);
        }

        public void GenerateQRCode(object parameter)
        {
            if (!string.IsNullOrEmpty(TextToEncode))
            {
                QRCodeEncoder encoder = new QRCodeEncoder();
                Bitmap qrBitmap = encoder.Encode(TextToEncode);

                MemoryStream ms = new MemoryStream();
                qrBitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                ms.Position = 0;
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = ms;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();

                GeneratedQRCode = bitmapImage;
                IsQRCreated = true;
            }
            else
            {
                MessageBox.Show("Please enter text to encode.");
            }
        }

        public bool CanGenerateQRCode(object parameter)
        {
            return TextToEncode != null;
        }

        public void DecodeQRCodeFromFile(object parameter)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.png;*.jpeg;*.jpg;*.gif;*.bmp)|*.png;*.jpeg;*.jpg;*.gif;*.bmp|All files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    Bitmap qrImage = new Bitmap(openFileDialog.FileName);
                    QRCodeDecoder decoder = new QRCodeDecoder();
                    string decodedText = decoder.Decode(new QRCodeBitmapImage(qrImage));
                    DecodedText = decodedText;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        public void SaveQRCodeImage(object parameter)
        {
            if (GeneratedQRCode != null)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "PNG Image|*.png";
                if (saveFileDialog.ShowDialog() == true)
                {
                    using (FileStream fileStream = new FileStream(saveFileDialog.FileName, FileMode.Create))
                    {
                        PngBitmapEncoder encoder = new PngBitmapEncoder();
                        encoder.Frames.Add(BitmapFrame.Create(GeneratedQRCode));
                        encoder.Save(fileStream);
                    }
                }
            }
        }

        public bool CanSaveQRCodeImage(object parameter)
        {
            return GeneratedQRCode != null;
        }

        public void SwitchToCreateQR(object parameter)
        {
            IsCreateQRSelected = true;
            IsScanQRSelected = false;
            if (GeneratedQRCode != null)
            {
                IsQRCreated = true;
            }
        }

        public void SwitchToScanQR(object parameter)
        {
            IsCreateQRSelected = false;
            IsScanQRSelected = true;
            IsQRCreated = false;
        }

        public void ShareQRCode(object parameter)
        {
            try
            {
                if (GeneratedQRCode != null)
                {
                    string tempFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "QRCodeImages");
                    if (!Directory.Exists(tempFolderPath))
                    {
                        Directory.CreateDirectory(tempFolderPath);
                    }

                    string randomFileName = Path.Combine(tempFolderPath, $"QRCode_{Guid.NewGuid()}.png");

                    using (FileStream fileStream = new FileStream(randomFileName, FileMode.Create))
                    {
                        PngBitmapEncoder encoder = new PngBitmapEncoder();
                        encoder.Frames.Add(BitmapFrame.Create(GeneratedQRCode));
                        encoder.Save(fileStream);
                    }

                    if (File.Exists(randomFileName))
                    {
                        var dataObject = new DataObject();
                        dataObject.SetFileDropList(new System.Collections.Specialized.StringCollection { randomFileName });

                        Clipboard.Clear();
                        Clipboard.SetDataObject(dataObject, true);

                        MessageBox.Show("QR Code image is saved and copied to clipboard. You can now paste it to share.");
                    }
                    else
                    {
                        MessageBox.Show("Failed to generate QR code image.");
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle the exception
                Console.WriteLine(ex.Message);
            }
        }

        public bool CanShareQRCode(object parameter)
        {
            return GeneratedQRCode != null;
        }

        public void CopyDecodedText(object parameter)
        {
            if (!string.IsNullOrEmpty(DecodedText))
            {
                Clipboard.SetText(DecodedText);
                MessageBox.Show("Decoded text copied to clipboard.");
            }
        }

        public bool CanCopyDecodedText(object parameter)
        {
            return !string.IsNullOrEmpty(DecodedText);
        }
    }
}
