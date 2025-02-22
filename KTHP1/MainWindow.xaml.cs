using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using KTHP1.Models;

namespace KTHP1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        QlbanHangContext db = new QlbanHangContext();

        private void HienThiDuLieu()
        {
            var sanPhamQuery = from sp in db.SanPhams
                               orderby sp.DonGia
                               select new
                               {
                                   sp.MaSp,
                                   sp.TenSp,
                                   sp.MaLoaiNavigation.TenLoai,
                                   sp.SoLuong,
                                   sp.DonGia,
                                   ThanhTien = sp.DonGia * sp.SoLuong
                               };

            dtgDSSP.ItemsSource = sanPhamQuery.ToList();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            HienThiDuLieu();

            var queryCB = from lsp in db.LoaiSanPhams
                          select lsp;
            cbTML.ItemsSource = queryCB.ToList();
            cbTML.DisplayMemberPath = "TenLoai";
            cbTML.SelectedValuePath = "MaLoai";
            cbTML.SelectedIndex = 0;
        }

        private void btnThem_Click(object sender, RoutedEventArgs e)
        {
            SanPham sp = new SanPham();

            if (isCheck())
            {
                sp.MaSp = txtMSP.Text;
                sp.TenSp = txtTSP.Text;
                sp.SoLuong = int.Parse(txtSL.Text);
                sp.DonGia = int.Parse(txtDG.Text);
                sp.MaLoai = cbTML.SelectedValue.ToString();

                db.SanPhams.Add(sp);
                db.SaveChanges();
                HienThiDuLieu();
            }
        }

        private void btnSua_Click(object sender, RoutedEventArgs e)
        {
             var querySua = from sp in db.SanPhams
                            where sp.MaSp == txtMSP.Text
                            select sp;

            SanPham spSua = querySua.FirstOrDefault();
            if (spSua != null) 
            {
                if (isCheck())
                {
                    spSua.TenSp = txtTSP.Text;
                    spSua.SoLuong = int.Parse(txtSL.Text);
                    spSua.DonGia = int.Parse(txtDG.Text);
                    spSua.MaLoai = cbTML.SelectedValue.ToString(); 
                }
            }

            db.SaveChanges() ;

            HienThiDuLieu() ;
        }

        private void btnXoa_Click(object sender, RoutedEventArgs e)
        {
            var queryXoa = from sp in db.SanPhams
                           where sp.MaSp == txtMSP.Text
                           select sp;

            SanPham spXoa = queryXoa.FirstOrDefault();

            if (spXoa != null)
            {
                MessageBoxResult tl = MessageBox.Show("Bạn có muốn xóa sản phẩm này không?", "Xóa", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (tl == MessageBoxResult.Yes)
                {
                    db.SanPhams.Remove(spXoa);
                    db.SaveChanges();
                    HienThiDuLieu();
                }
            }else
            {
                MessageBox.Show("Không có sản phẩm muốn xóa","Xóa",MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void btnTim_Click(object sender, RoutedEventArgs e)
        {
            var queryTim = from sp in db.SanPhams
                           join lsp in db.LoaiSanPhams
                           on sp.MaLoai equals lsp.MaLoai 
                           group sp by lsp.TenLoai into nhom
                           select new
                           {
                               TenLoai = nhom.Key,
                               TongSoLuong = nhom.Sum(sp => sp.SoLuong),
                               TongTien = nhom.Sum(sp => sp.SoLuong * sp.DonGia)
                           };

            Window1 myWin = new Window1();
            myWin.dtgTim.ItemsSource = queryTim.ToList();
            myWin.ShowDialog();
        }

        private bool isCheck()
        {
            if(txtMSP.Text == "")
            {
                MessageBox.Show("Bạn chưa nhập mã sản phẩm","Lỗi nhập dữ liệu",MessageBoxButton.OK, MessageBoxImage.Error);
                txtMSP.Focus();
                return false;
            }
            if (txtTSP.Text == "")
            {
                MessageBox.Show("Bạn chưa tên mã sản phẩm", "Lỗi nhập dữ liệu", MessageBoxButton.OK, MessageBoxImage.Error);
                txtTSP.Focus();
                return false;
            }
            if (txtSL.Text == "" || (!int.TryParse(txtSL.Text,out int soLuong) || soLuong <= 0) )
            {
                MessageBox.Show("Số lượng phải là số nguyên dương", "Lỗi nhập dữ liệu", MessageBoxButton.OK, MessageBoxImage.Error);
                txtSL.Focus();
                return false;
            }
            if (txtDG.Text == "" || (!int.TryParse(txtDG.Text, out int donGia) || donGia <= 0))
            {
                MessageBox.Show("Đơn giá phải là số nguyên dương", "Lỗi nhập dữ liệu", MessageBoxButton.OK, MessageBoxImage.Error);
                txtDG.Focus();
                return false;
            }
            return true;
        }

        private void dtgDSSP_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dtgDSSP.SelectedItem != null)
            {
                var dongChon = (dynamic)dtgDSSP.SelectedItem;

                txtMSP.Text = dongChon.MaSp;
                txtTSP.Text = dongChon.TenSp;
                txtSL.Text = dongChon.SoLuong.ToString();
                txtDG.Text = dongChon.DonGia.ToString();
                cbTML.Text = dongChon.TenLoai;
            }
        }
    }
}