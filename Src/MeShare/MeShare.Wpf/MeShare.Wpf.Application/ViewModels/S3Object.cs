using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Prism.Mvvm;

namespace MeShare.Wpf.Application.ViewModels
{
    public class S3Object : BindableBase,IDisposable
    {

        private BackgroundWorker bw;
        private string _fileName;
        private string _uploaded;
        private string _fileSize;
        private int _process;
        private string _status;
        private readonly AmazonS3Client _client;
        private  CancellationTokenSource _tokenSource;

        public S3Object()
        {
            _tokenSource = new CancellationTokenSource();
            AwsBucket = ConfigurationManager.AppSettings["AwsBucket"];
            _client = new AmazonS3Client(Amazon.RegionEndpoint.EUWest1);
            bw = new BackgroundWorker();
            bw.DoWork += Bw_DoWork;
            bw.RunWorkerCompleted += Bw_RunWorkerCompleted;
        }

        public string AwsBucket { get; set; }

        public string FileName
        {
            get { return _fileName; }
            set
            {
                _fileName = value;
                RaisePropertyChanged();
            }
        }

        private void Bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Status = _tokenSource.IsCancellationRequested ? "Stopped" : "Uploaded";
        }

        private void Bw_DoWork(object sender, DoWorkEventArgs e)
        {
            var file = new FileInfo(FileName);
            FileSize = file.Length.ToString();
            Status = "Uploading";

            using (var fileTransferUtility = new TransferUtility(_client))
            {
                var uploadRequest =
                    new TransferUtilityUploadRequest
                    {
                        BucketName = AwsBucket,
                        FilePath = file.FullName,
                        Key = $"tmp/{file.Name}",
                        CannedACL = S3CannedACL.Private,
                        ContentType = MimeMapping.GetMimeMapping(file.Name),
                        TagSet = new List<Tag>()
                        {
                            new Tag()
                            {
                                Key = "MachineName",
                                Value = Environment.MachineName
                            },
                            new Tag()
                            {
                                Key = "UserName",
                                Value = Environment.UserName
                            },
                        }
                    };


                uploadRequest.UploadProgressEvent += new EventHandler<UploadProgressArgs>(uploadRequest_UploadPartProgressEvent);


                //fileTransferUtility.Upload(uploadRequest);
                fileTransferUtility.UploadAsync(uploadRequest, _tokenSource.Token);
            }
        }

        private void uploadRequest_UploadPartProgressEvent(object sender, UploadProgressArgs e)
        {
            Process = e.PercentDone;
        }

        public int Process
        {
            get { return _process; }
            set { _process = value; RaisePropertyChanged(); }
        }

        public string Status
        {
            get { return _status; }
            set { _status = value; RaisePropertyChanged(); }
        }

        public string FileSize
        {
            get { return _fileSize; }
            set { _fileSize = value; RaisePropertyChanged(); }
        }

        public async Task Start()
        {
            await Task.Run(() =>
            {
                if (bw.IsBusy != true)
                {
                    _tokenSource = new CancellationTokenSource();
                    bw.RunWorkerAsync();
                }
            });
        }

        public string GetUrl()
        {
            FileInfo file = null;
            file = new FileInfo(FileName);

            GetPreSignedUrlRequest requestOrg = new GetPreSignedUrlRequest
            {
                BucketName = AwsBucket,
                Key = string.Format("tmp/{0}", file.Name),
                Expires = DateTime.Now.AddMinutes(60)
            };

            return _client.GetPreSignedURL(requestOrg);
        }

        public void CancelUpload()
        {
            _tokenSource.Cancel();
        }

        public void Dispose()
        {
            _client.Dispose();
            bw.Dispose();
        }
    }
}