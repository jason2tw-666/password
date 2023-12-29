using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace passsword_tools
{
    //當前版本1.0.2
    //既有障礙:目前第一次複製完畢時，再次複製會再詢問一次匯入檔案。
    //既有障礙:選擇所需的帳號目前實體檔可以塞多支帳號，但是要能夠多其他清單必須要由程式內部調整
    public partial class Form1 : Form
    {
        [DllImport("user32.dll", SetLastError = true)]
        public static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

        // 定义Ctrl键的虚拟键码
        private const int VK_CONTROL = 0x11;

        // 定义Ctrl键的按下和释放标志位
        private const int KEYEVENTF_KEYDOWN = 0x0;
        private const int KEYEVENTF_KEYUP = 0x2;

        public Form1()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

            //4564

        }


        private void label2_Click(object sender, EventArgs e)
        {

        }
        
        public class UserCredential
        {
            public string AD { get; set; }
            public string PW { get; set; }
            public string VC { get; set; }
        }

        public List<UserCredential> userCredentials = new List<UserCredential>();


        private void button2_Click(object sender, EventArgs e)
        {

            // 创建 OpenFileDialog 实例
            OpenFileDialog openFileDialog = new OpenFileDialog();

            // 设置对话框的标题
            openFileDialog.Title = "選擇要匯入的文件";

            // 设置对话框可以选择的文件类型
            openFileDialog.Filter = "Json files (*.json)|*.json|Text files (*.txt)|*.txt";

            // 如果用户点击了“确定”按钮
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {

                    // 讀取JSON文件內容
                    string jsonContent = File.ReadAllText(openFileDialog.FileName);
                    // 將JSON字串解析為JObject
                    userCredentials = JsonConvert.DeserializeObject<List<UserCredential>>(jsonContent);
                    // 存取解析後的物件內容
                    foreach (var credential in userCredentials)
                    {
                        comboBox1.Items.Add(credential.AD);
  
                    }
                    MessageBox.Show("文件導入成功！", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"文件導入失败：{ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    comboBox1.Enabled = false;
                }
                comboBox1.Enabled = true;
            }
            
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            foreach (var credential in userCredentials)
            {
                if (comboBox1.SelectedItem.ToString() == credential.AD) {
                    textBox1.Text = credential.AD;
                    textBox2.Text = credential.PW;
                    textBox3.Text = credential.VC;
                }
            }
        }

        //共用複製功能
        private void CopyData(string name)
        {
            Clipboard.SetText(name);
       
        }

        //帳號複製按鈕
        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != "") CopyData(textBox1.Text);

        }
        //密碼複製按鈕
        private void button8_Click(object sender, EventArgs e)
        {
            if(textBox2.Text!="")CopyData(textBox2.Text);
        }
        //驗整碼複製按鈕
        private void button5_Click(object sender, EventArgs e)
        {
            if (textBox3.Text != "") {
                CopyData(textBox3.Text);
                //button3_Click("", e);
            } 
        }
        //自訂複製按鈕
        private void button7_Click(object sender, EventArgs e)
        {
            //textBox4.SelectedText
            if (textBox4.SelectedText != "") CopyData(textBox4.Text);
        }
        //驗證碼欄位
        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }
        //帳號欄位
        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
        //密碼欄位
        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }


        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
        
        //設置剩餘時間為 58 分鐘
        private int countdownSeconds = 3480;
        private bool isButtonEnabled = true;
        private void button3_Click(object sender, EventArgs e)
        {
            //二次點擊功能觸發Refresh
            if (!isButtonEnabled)
            {
                RefreshTimer();
                return;
            }
            {
                InitializeComponent();
               
                if (isButtonEnabled)
                {
                    // 设置 Timer 的时间间隔为1000毫秒（1秒）
                    timer1.Interval = 1000;
                    // 啟動计时器
                    timer1.Start();
                    isButtonEnabled = false;
                    timer1_Tick("",e);   
                } 
                return;
            }
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            var Retrytimmer = false;

            timeforworkdata(Retrytimmer);

            //判斷是否需要走Retry機制
            if (Retrytimmer){isButtonEnabled = true;}
            else{isButtonEnabled = false;}
        }

        private void timeforworkdata (bool check)
        {
            // 每秒鐘减少剩餘时间
            countdownSeconds--;

            // 更新顯示剩餘时间
            UpdateCountdownLabel();

            // 如果倒计时结束，停止计时器
            if (countdownSeconds <= 0)
            {
                timer1.Stop();
                isButtonEnabled = true;
                label5.Text = $"當前時間：{DateTime.Now.ToString("HH:mm:ss")}";
                MessageBox.Show("-------時間快都搂！-------" + "\n\n\n" + "剩餘2分鐘", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                label5.Text = "";
                //因程式未重啟，固定結束吃回原本變數58分鐘
                countdownSeconds = 3480;
                check = true;
                return;
            }
            check = false;
        }

        private void UpdateCountdownLabel()
        {
            // 将剩余时间格式化为 HH:mm:ss
            TimeSpan timeSpan = TimeSpan.FromSeconds(countdownSeconds);
            label5.Text = $"剩餘時間：{timeSpan.ToString(@"hh\:mm\:ss")}";
        }

        private void RefreshTimer()
        {
            // 重新設置剩餘時間為 58 分鐘
            countdownSeconds = 3480;

            // 啟動计时器
            timer1.Start();

            // 更新显示剩餘時間
            UpdateCountdownLabel();          
        }
        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }





    }
}
