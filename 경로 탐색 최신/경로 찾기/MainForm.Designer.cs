namespace 경로_찾기
{
    partial class MainForm
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.btn_exit = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.btn_search = new System.Windows.Forms.Button();
            this.tb_total_distance = new System.Windows.Forms.TextBox();
            this.btn_simulation = new System.Windows.Forms.Button();
            this.lbox_end = new System.Windows.Forms.ListBox();
            this.btn_end = new System.Windows.Forms.Button();
            this.tb_end_lng = new System.Windows.Forms.TextBox();
            this.tb_end_lat = new System.Windows.Forms.TextBox();
            this.lbox_start = new System.Windows.Forms.ListBox();
            this.btn_start = new System.Windows.Forms.Button();
            this.tb_start_lng = new System.Windows.Forms.TextBox();
            this.tb_start_lat = new System.Windows.Forms.TextBox();
            this.lbox_point = new System.Windows.Forms.ListBox();
            this.webbr = new System.Windows.Forms.WebBrowser();
            this.timer_simulation = new System.Windows.Forms.Timer(this.components);
            this.tb_start_name = new System.Windows.Forms.TextBox();
            this.tb_end_name = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.tb_end_name);
            this.splitContainer1.Panel1.Controls.Add(this.tb_start_name);
            this.splitContainer1.Panel1.Controls.Add(this.btn_exit);
            this.splitContainer1.Panel1.Controls.Add(this.label1);
            this.splitContainer1.Panel1.Controls.Add(this.btn_search);
            this.splitContainer1.Panel1.Controls.Add(this.tb_total_distance);
            this.splitContainer1.Panel1.Controls.Add(this.btn_simulation);
            this.splitContainer1.Panel1.Controls.Add(this.lbox_end);
            this.splitContainer1.Panel1.Controls.Add(this.btn_end);
            this.splitContainer1.Panel1.Controls.Add(this.tb_end_lng);
            this.splitContainer1.Panel1.Controls.Add(this.tb_end_lat);
            this.splitContainer1.Panel1.Controls.Add(this.lbox_start);
            this.splitContainer1.Panel1.Controls.Add(this.btn_start);
            this.splitContainer1.Panel1.Controls.Add(this.tb_start_lng);
            this.splitContainer1.Panel1.Controls.Add(this.tb_start_lat);
            this.splitContainer1.Panel1.Controls.Add(this.lbox_point);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.webbr);
            this.splitContainer1.Size = new System.Drawing.Size(1516, 599);
            this.splitContainer1.SplitterDistance = 456;
            this.splitContainer1.TabIndex = 0;
            // 
            // btn_exit
            // 
            this.btn_exit.Font = new System.Drawing.Font("굴림", 36F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btn_exit.Location = new System.Drawing.Point(360, 554);
            this.btn_exit.Name = "btn_exit";
            this.btn_exit.Size = new System.Drawing.Size(205, 97);
            this.btn_exit.TabIndex = 11;
            this.btn_exit.Text = "종료";
            this.btn_exit.UseVisualStyleBackColor = true;
            this.btn_exit.Click += new System.EventHandler(this.btn_exit_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 366);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 10;
            this.label1.Text = "총 거리 :";
            // 
            // btn_search
            // 
            this.btn_search.Font = new System.Drawing.Font("굴림", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btn_search.Location = new System.Drawing.Point(359, 391);
            this.btn_search.Name = "btn_search";
            this.btn_search.Size = new System.Drawing.Size(205, 157);
            this.btn_search.TabIndex = 5;
            this.btn_search.Text = "경로\r\n찾기";
            this.btn_search.UseVisualStyleBackColor = true;
            this.btn_search.Click += new System.EventHandler(this.btn_search_Click);
            // 
            // tb_total_distance
            // 
            this.tb_total_distance.Enabled = false;
            this.tb_total_distance.Location = new System.Drawing.Point(72, 363);
            this.tb_total_distance.Name = "tb_total_distance";
            this.tb_total_distance.Size = new System.Drawing.Size(492, 21);
            this.tb_total_distance.TabIndex = 9;
            // 
            // btn_simulation
            // 
            this.btn_simulation.Location = new System.Drawing.Point(359, 553);
            this.btn_simulation.Name = "btn_simulation";
            this.btn_simulation.Size = new System.Drawing.Size(87, 33);
            this.btn_simulation.TabIndex = 8;
            this.btn_simulation.Text = "시뮬레이션";
            this.btn_simulation.UseVisualStyleBackColor = true;
            this.btn_simulation.Click += new System.EventHandler(this.btn_simulation_Click);
            // 
            // lbox_end
            // 
            this.lbox_end.FormattingEnabled = true;
            this.lbox_end.ItemHeight = 12;
            this.lbox_end.Location = new System.Drawing.Point(14, 221);
            this.lbox_end.Name = "lbox_end";
            this.lbox_end.Size = new System.Drawing.Size(550, 112);
            this.lbox_end.TabIndex = 6;
            // 
            // btn_end
            // 
            this.btn_end.Location = new System.Drawing.Point(14, 185);
            this.btn_end.Name = "btn_end";
            this.btn_end.Size = new System.Drawing.Size(60, 23);
            this.btn_end.TabIndex = 4;
            this.btn_end.Text = "목적지";
            this.btn_end.UseVisualStyleBackColor = true;
            this.btn_end.Click += new System.EventHandler(this.btn_end_Click);
            // 
            // tb_end_lng
            // 
            this.tb_end_lng.Enabled = false;
            this.tb_end_lng.Location = new System.Drawing.Point(342, 187);
            this.tb_end_lng.Name = "tb_end_lng";
            this.tb_end_lng.Size = new System.Drawing.Size(111, 21);
            this.tb_end_lng.TabIndex = 3;
            // 
            // tb_end_lat
            // 
            this.tb_end_lat.Enabled = false;
            this.tb_end_lat.Location = new System.Drawing.Point(214, 187);
            this.tb_end_lat.Name = "tb_end_lat";
            this.tb_end_lat.Size = new System.Drawing.Size(122, 21);
            this.tb_end_lat.TabIndex = 3;
            // 
            // lbox_start
            // 
            this.lbox_start.FormattingEnabled = true;
            this.lbox_start.ItemHeight = 12;
            this.lbox_start.Location = new System.Drawing.Point(13, 49);
            this.lbox_start.Name = "lbox_start";
            this.lbox_start.Size = new System.Drawing.Size(551, 124);
            this.lbox_start.TabIndex = 2;
            // 
            // btn_start
            // 
            this.btn_start.Location = new System.Drawing.Point(12, 20);
            this.btn_start.Name = "btn_start";
            this.btn_start.Size = new System.Drawing.Size(62, 23);
            this.btn_start.TabIndex = 1;
            this.btn_start.Text = "출발지";
            this.btn_start.UseVisualStyleBackColor = true;
            this.btn_start.Click += new System.EventHandler(this.btn_start_Click);
            // 
            // tb_start_lng
            // 
            this.tb_start_lng.Enabled = false;
            this.tb_start_lng.Location = new System.Drawing.Point(342, 22);
            this.tb_start_lng.Name = "tb_start_lng";
            this.tb_start_lng.Size = new System.Drawing.Size(112, 21);
            this.tb_start_lng.TabIndex = 0;
            // 
            // tb_start_lat
            // 
            this.tb_start_lat.Enabled = false;
            this.tb_start_lat.Location = new System.Drawing.Point(214, 22);
            this.tb_start_lat.Name = "tb_start_lat";
            this.tb_start_lat.Size = new System.Drawing.Size(122, 21);
            this.tb_start_lat.TabIndex = 0;
            // 
            // lbox_point
            // 
            this.lbox_point.FormattingEnabled = true;
            this.lbox_point.ItemHeight = 12;
            this.lbox_point.Location = new System.Drawing.Point(14, 391);
            this.lbox_point.Name = "lbox_point";
            this.lbox_point.Size = new System.Drawing.Size(340, 268);
            this.lbox_point.TabIndex = 7;
            this.lbox_point.SelectedIndexChanged += new System.EventHandler(this.lbox_point_SelectedIndexChanged);
            // 
            // webbr
            // 
            this.webbr.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webbr.Location = new System.Drawing.Point(0, 0);
            this.webbr.MinimumSize = new System.Drawing.Size(20, 20);
            this.webbr.Name = "webbr";
            this.webbr.Size = new System.Drawing.Size(1056, 599);
            this.webbr.TabIndex = 0;
            this.webbr.Url = new System.Uri("https://kwonminsugithub.github.io/ms2/index.html", System.UriKind.Absolute);
            // 
            // timer_simulation
            // 
            this.timer_simulation.Tick += new System.EventHandler(this.timer_simulation_Tick);
            // 
            // tb_start_name
            // 
            this.tb_start_name.Enabled = false;
            this.tb_start_name.Location = new System.Drawing.Point(80, 22);
            this.tb_start_name.Name = "tb_start_name";
            this.tb_start_name.Size = new System.Drawing.Size(128, 21);
            this.tb_start_name.TabIndex = 12;
            // 
            // tb_end_name
            // 
            this.tb_end_name.Enabled = false;
            this.tb_end_name.Location = new System.Drawing.Point(80, 187);
            this.tb_end_name.Name = "tb_end_name";
            this.tb_end_name.Size = new System.Drawing.Size(128, 21);
            this.tb_end_name.TabIndex = 12;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1516, 599);
            this.Controls.Add(this.splitContainer1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "MainForm";
            this.Text = "경로 찾기";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Button btn_search;
        private System.Windows.Forms.Button btn_end;
        private System.Windows.Forms.TextBox tb_end_lat;
        private System.Windows.Forms.ListBox lbox_start;
        private System.Windows.Forms.Button btn_start;
        private System.Windows.Forms.TextBox tb_start_lat;
        private System.Windows.Forms.Button btn_simulation;
        private System.Windows.Forms.ListBox lbox_point;
        private System.Windows.Forms.ListBox lbox_end;
        private System.Windows.Forms.WebBrowser webbr;
        private System.Windows.Forms.Timer timer_simulation;
        private System.Windows.Forms.TextBox tb_total_distance;
        private System.Windows.Forms.TextBox tb_start_lng;
        private System.Windows.Forms.TextBox tb_end_lng;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btn_exit;
        private System.Windows.Forms.TextBox tb_end_name;
        private System.Windows.Forms.TextBox tb_start_name;
    }
}

