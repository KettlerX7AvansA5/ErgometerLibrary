using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ErgometerLibrary.Chat
{
    public class ChatItem : Panel
    {
        private Label messageLabel, timeLabel;

        public ChatItem(ChatMessage chat) : base()
        {
            this.messageLabel = new Label();
            this.timeLabel = new Label();

            this.AutoSize = true;
            this.setAppearance(chat.IsDoctor);
            this.Controls.Add(this.messageLabel);
            this.Controls.Add(this.timeLabel);
            this.MinimumSize = new System.Drawing.Size(150, 60);
            this.Name = "messageContainer";
            this.Padding = new System.Windows.Forms.Padding(6);

            //
            // Message Label
            // 
            this.messageLabel.AutoSize = true;
            this.messageLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.messageLabel.Font = new System.Drawing.Font("Segoe UI Semibold", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.messageLabel.Location = new System.Drawing.Point(6, 6);
            this.messageLabel.MaximumSize = new System.Drawing.Size(250, 0);
            this.messageLabel.Size = new System.Drawing.Size(121, 20);
            this.messageLabel.Text = chat.Message.Replace("\n", "");
            // 
            // Time Label
            // 
            this.timeLabel.AutoSize = true;
            this.timeLabel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.timeLabel.Font = new System.Drawing.Font("Segoe UI Semilight", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.timeLabel.Location = new System.Drawing.Point(6, 26);
            this.timeLabel.Margin = new System.Windows.Forms.Padding(6);
            this.timeLabel.Size = new System.Drawing.Size(32, 13);
            this.timeLabel.Text = Helper.MillisecondsToTime(chat.TimeStamp);
        }

        private void setAppearance(bool isDoctor)
        {
            if (isDoctor)
            {
                this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
                this.Dock = DockStyle.Right;
            }
            else
            {
                this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
                this.Dock = DockStyle.Left;
            }
        }
    }
}
