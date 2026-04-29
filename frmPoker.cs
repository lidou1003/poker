using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace poker
{
    public partial class frmPoker : Form
    {
        #region 欄位
        /// <summary>
        /// 用來存放牌桌上五張牌的 PictureBox 陣列。
        /// </summary>
        PictureBox[] pic = new PictureBox[5];

        /// <summary>
        /// 所有撲克牌的編號陣列，從 0 到 51，對應到 52 張牌。
        /// </summary>
        int[] allPoker = new int[52];

        /// <summary>
        /// 紀錄玩家手牌的編號陣列，從 0 到 51，對應到 52 張牌。
        /// </summary>
        int[] playerPoker = new int[5];

        // 玩家目前總資金
        int money = 1000000;

        // 這一局押注金額
        int betMoney = 0;

        #endregion

        public frmPoker()
        {
            InitializeComponent();
            InitializePoker();

            txtMoney.Text = money.ToString();

            btnDealCard.Enabled = false;
            btnChangeCard.Enabled = false;
            btnCheck.Enabled = false;
        }

        #region 自定義方法
        private void InitializePoker()
        {
            for (int i = 0; i < pic.Length; i++)
            {
                pic[i] = new PictureBox();
                pic[i].Image = GetImage("back");
                pic[i].Name = "pic" + i;
                pic[i].SizeMode = PictureBoxSizeMode.AutoSize;
                pic[i].Top = 30;
                pic[i].Left = 10 + ((pic[i].Width + 10) * i);
                pic[i].Visible = true;
                pic[i].Enabled = false;
                pic[i].Tag = "back";

                // 將 pic 丟至到 grpPoker 內
                this.grpPoker.Controls.Add(pic[i]);

                pic[i].Click += Pic_Click;
            }
        }

        /// <summary>
        /// 顯示五張撲克牌到桌面上
        /// </summary>
        private void ShowCards()
        {
            for (int i = 0; i < playerPoker.Length; i++)
            {
                pic[i].Image = this.GetImage($"pic{playerPoker[i] + 1}");
            }
        }



        /// <summary>
        /// 取得圖片資源
        /// </summary>
        /// <param name="name"> string 的牌名</param>
        /// <returns></returns>
        private Image GetImage(string name)
        {
            return Properties.Resources.ResourceManager.GetObject(name) as Image;
        }

        /// <summary>
        /// 取得圖片資源
        /// </summary>
        /// <param name="num">撲克牌編號</param>
        /// <returns></returns>
        private Image GetImage(int num)
        {
            return GetImage($"pic{num}");
        }


        /// <summary>
        /// 將52張牌打亂
        /// </summary>
        private void Shuffle()
        {
            Random rand = new Random();
            for (int i = 0; i < 2000; i++)
            {
                int r = rand.Next(allPoker.Length);
                int temp = allPoker[r];
                allPoker[r] = allPoker[0];
                allPoker[0] = temp;
            }
        }
        #endregion


        #region 事件處理程序


        /// <summary>
        /// 牌桌上每張牌的 Click 事件處理程序，當按下牌時，會顯示該牌的名稱。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Pic_Click(object sender, EventArgs e)
        {
            PictureBox pic = sender as PictureBox;

            int index = int.Parse(pic.Name.Replace("pic", ""));

            int cardNum = playerPoker[index] + 1;

            // 如果牌是背面朝上，則翻開牌面；如果牌是正面朝上，則翻回背面。
            if (pic.Tag.ToString() == "back")
            {
                pic.Tag = "front";
                pic.Image = GetImage(cardNum);
            }
            else
            {
                pic.Tag = "back";
                pic.Image = GetImage("back");
            }

            //MessageBox.Show($"牌編號{cardNum}");
        }

        /// <summary>
        /// 當按下發牌按鈕時，會隨機產生五個1~52的數字，並將對應的圖片顯示在牌桌上。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnDealCard_Click(object sender, EventArgs e)
        {

            // 將 lblResult 的文字清空
            this.lblResult.Text = $"本局押注：{betMoney} 元";


            /// 將牌桌上的圖片重置為背面圖
            for (int i = 0; i < pic.Length; i++)
            {
                pic[i].Image = GetImage("back");
            }

            // 將所有撲克牌的編號從 0 到 51 填入 allPoker 陣列
            for (int i = 0; i < allPoker.Length; i++)
            {
                allPoker[i] = i;
            }

            // 洗牌
            this.Shuffle();


            // 測試
            playerPoker[0] = 51;
            playerPoker[1] = 47;
            playerPoker[2] = 43;
            playerPoker[3] = 39;
            playerPoker[4] = 3;


            // 暫停500ms
            await Task.Delay(500);

            // 發牌
            for (int i = 0; i < playerPoker.Length; i++)
            {
                // 取前52張牌的前五張牌
                playerPoker[i] = allPoker[i];
            }

            // 測試
            //playerPoker[0] = 48;
            //playerPoker[1] = 39;
            //playerPoker[2] = 15;
            //playerPoker[3] = 14;
            //playerPoker[4] = 13;


            // 將對應的牌面圖顯示在牌桌上
            this.ShowCards();

            // 啟用所有牌的點擊事件
            for (int i = 0; i < pic.Length; i++)
            {
                // 將牌桌上的牌設成可以點擊
                pic[i].Enabled = true;
                // 將牌桌上的牌的 Tag 設成 "front"，表示這些牌是正面朝上
                pic[i].Tag = "front";
            }
            // 啟用換牌按鈕
            btnChangeCard.Enabled = true;
            btnDealCard.Enabled = false;

        }

        /// <summary>
        /// 當按下換牌按鈕時，會將玩家選擇的牌換成新的牌，並將對應的圖片顯示在牌桌上。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnChangeCard_Click(object sender, EventArgs e)
        {
            int startIndex = 5;
            
            for(int i = 0; i < playerPoker.Length; i++)
            {
                if (pic[i].Tag.ToString() == "back")
                {
                    // 如果牌是背面朝上，則換牌
                    playerPoker[i] = allPoker[startIndex];
                    startIndex++;
                    // 將對應的牌面圖顯示在牌桌上
                    pic[i].Image = GetImage(playerPoker[i] + 1);
                    pic[i].Tag = "front";
                }
            }

            for (int i = 0; i < pic.Length; i++)
            {
                // 將牌桌上的牌設成不能點擊
                pic[i].Enabled = false;
            }

            this.btnChangeCard.Enabled = false;

            // 啟用檢查牌型按鈕
            this.btnCheck.Enabled = true;
        }

        /// <summary>
        /// 當按下判斷牌型按鈕時，會判斷玩家手牌的牌型，並顯示在 lblResult 上。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCheck_Click(object sender, EventArgs e)
        {
            string[] colorList = { "梅花", "方塊", "愛心", "黑桃" };
            string[] pointList = { "A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q","K" };

            // 紀錄目前五張撲克牌的花色的陣列
            int[] pokerColor = new int[5];
            // 紀錄目前五張撲克牌的點數的陣列
            int[] pokerPoint = new int[5];


            for (int i = 0; i < playerPoker.Length; i++)
            {
                // 根據玩家手牌的編號，計算出該牌的花色和點數
                pokerColor[i] = playerPoker[i] % 4;
                pokerPoint[i] = playerPoker[i] / 4;
            }

            #region 測試計算出的花色和點數是否正確
            //==============================================================================
            //string result = "玩家: ";
            //for(int i = 0; i < playerPoker.Length; i++)
            //{
            // 取得牌的花色和點數
            //   int iColor = pokerColor[i];
            //   int iPoint = pokerPoint[i];
            // 根據花色編號和點數編號，組合成牌的名稱
            //   result += $"{colorList[iColor]}{pointList[iPoint]} ";
            //}

            // 顯示玩家撲克牌的花色和點數
            //this.lblResult.Text = result;
            //==============================================================================

            #endregion

            // 紀錄花色和點數出現的次數的陣列
            int[] colorCount = new int[4];
            int[] pointCount = new int[13];

            // 統計 Color 和 Point 出現的次數
            for (int i = 0; i < pokerColor.Length; i++)
            {
                int color = pokerColor[i];
                int point = pokerPoint[i];

                colorCount[color]++;
                pointCount[point]++;
            }

            // 將花色和點數的次數陣列進行排序，讓出現次數多的花色和點數排在前面
            Array.Sort(colorCount, colorList);
            Array.Reverse(colorCount);
            Array.Reverse(colorList);

            Array.Sort(pointCount, pointList);
            Array.Reverse(pointCount);
            Array.Reverse(pointList);


            // 判斷是否為同花
            bool isFlush = (colorCount[0] == 5);
            // 判斷是否為五張單張
            bool isSingle = (pointCount[0] == 1 && pointCount[1] == 1 && pointCount[2] == 1 &&
            pointCount[3] == 1 && pointCount[4] == 1);
            // 判斷是否為差四
            bool isDiffFout = (pokerPoint.Max() - pokerPoint.Min() == 4);
            // 判斷是否為大順
            bool isRoyal = pokerPoint.Contains(0) && pokerPoint.Contains(9) &&
            pokerPoint.Contains(10) && pokerPoint.Contains(11) && pokerPoint.Contains(12);
            // 判斷是否為同花大順
            bool isRoyalisFlush = isFlush && isRoyal;
            // 判斷是否為同花順
            bool isStraightFlush = isFlush && isSingle && isDiffFout;
            // 判斷是否為順子
            bool isStraight = isSingle && (isDiffFout || isRoyal);
            // 判斷是否為鐵支
            bool isFourOfAKind = (pointCount[0] == 4);
            // 判斷是否為葫蘆
            bool isFullHouse = (pointCount[0] == 3 && pointCount[1] == 2);
            // 判斷是否為三條
            bool isThreeOfAKind = (pointCount[0] == 3 && pointCount[1] == 1);
            // 判斷是否為兩對
            bool isTwoPair = (pointCount[0] == 2 && pointCount[1] == 2);
            // 判斷是否為一對
            bool isOnePair = (pointCount[0] == 2 && pointCount[1] == 1);


            string result = "";
            int rate = 0;

            if (isRoyalisFlush)
            {
                result = $"{colorList[0]} 皇家同花順";
                rate = 250;
            }
            else if (isStraightFlush)
            {
                result = $"{colorList[0]} 同花順";
                rate = 50;
            }
            else if (isStraight)
            {
                result = "順子";
                rate = 4;
            }
            else if (isFourOfAKind)
            {
                result = $"{pointList[0]} 鐵支";
                rate = 25;

            }
            else if (isFullHouse)
            {
                result = $"{pointList[0]}三張{pointList[1]}兩張 葫蘆";
                rate = 9;
            }
            else if (isFlush)
            {
                result = $"{colorList[0]} 同花";
                rate = 6;
            }
            else if (isThreeOfAKind)
            {
                result = $"{pointList[0]} 三條";
                rate = 3;
            }
            else if (isTwoPair)
            {
                result = $"{pointList[0]},{pointList[1]} 兩對";
                rate = 2;
            }
            else if (isOnePair)
            {
                result = $"{pointList[0]} 一對";
                rate = 1;
            }
            else
            {
                result = "雜牌";
                rate = 0;
            }

            int winMoney = 0;

            if (rate > 0)
            {
                winMoney = betMoney * rate;
                money += winMoney;
                lblResult.Text = $"{result}，贏得 {winMoney} 元，目前總資金：{money}";
            }
            else
            {
                winMoney = betMoney;
                money -= winMoney;
                lblResult.Text = $"{result}，輸掉 {winMoney} 元，目前總資金：{money}";
            }

            txtMoney.Text = money.ToString();

            btnChangeCard.Enabled = false;
            btnCheck.Enabled = false;
            btnDealCard.Enabled = false;
            btnBet.Enabled = true;

            betMoney = 0;
        }

        /// <summary>
        /// 當表單被按下鍵盤時，顯示訊息框告訴使用者按下了哪個鍵。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmPoker_KeyPress(object sender, KeyPressEventArgs e)
        {
            // 如果目前游標在文字框裡，就不要執行測試牌功能
            if (this.ActiveControl is TextBox)
            {
                return;
            }

            if (this.btnDealCard.Enabled == false)
            {
                switch (e.KeyChar) {
                    case 'q':
                              // 同花大順
                        playerPoker[0] = 51;
                        playerPoker[1] = 47;
                        playerPoker[2] = 43;
                        playerPoker[3] = 39;
                        playerPoker[4] = 3;
                        break;
                    case 'w':
                              // 同花順
                        playerPoker[0] = 37;
                        playerPoker[1] = 33;
                        playerPoker[2] = 29;
                        playerPoker[3] = 25;
                        playerPoker[4] = 21;
                        break;
                    case 'e':
                              // 同花
                        playerPoker[0] = 50;
                        playerPoker[1] = 38;
                        playerPoker[2] = 34;
                        playerPoker[3] = 22;
                        playerPoker[4] = 18;
                        break;
                    case 'r':
                              // 鐵支
                        playerPoker[0] = 48;
                        playerPoker[1] = 39;
                        playerPoker[2] = 38;
                        playerPoker[3] = 37;
                        playerPoker[4] = 36;
                        break;
                    case 't':
                               // 葫蘆
                        playerPoker[0] = 30;
                        playerPoker[1] = 29;
                        playerPoker[2] = 6;
                        playerPoker[3] = 5;
                        playerPoker[4] = 4;
                        break;
                    case 'y':
                               // 三條
                        playerPoker[0] = 48;
                        playerPoker[1] = 39;
                        playerPoker[2] = 15;
                        playerPoker[3] = 14;
                        playerPoker[4] = 13;
                        break;
                }

                // 顯示五張撲克牌到桌面上
                ShowCards();
            }
        }

        /// <summary>
        /// 當按下押注鍵時，顯示訊息框給使用者看
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBet_Click(object sender, EventArgs e)
        {
            // 檢查押注金額是不是數字
            if (!int.TryParse(txtBet.Text, out betMoney))
            {
                MessageBox.Show("請輸入正確的押注金額！");
                return;
            }

            // 檢查押注金額是否大於 0
            if (betMoney <= 0)
            {
                MessageBox.Show("押注金額必須大於 0！");
                return;
            }

            // 檢查押注金額是否超過總資金
            if (betMoney > money)
            {
                MessageBox.Show("押注金額不能超過總資金！");
                return;
            }

            lblResult.Text = $"已押注 {betMoney} 元，請按發牌。";

            btnBet.Enabled = false;
            btnDealCard.Enabled = true;
            btnChangeCard.Enabled = false;
            btnCheck.Enabled = false;
        }


        #endregion

    }
}
