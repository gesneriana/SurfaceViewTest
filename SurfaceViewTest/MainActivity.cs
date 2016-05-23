using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Graphics;
using Java.Util;
using Android.Util;

namespace SurfaceViewTest
{
    [Activity(Label = "SurfaceViewTest", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity, View.IOnClickListener, ISurfaceHolderCallback
    {
        private string TAG = "调试信息";
        private ISurfaceHolder holder;
        private Paint paint;
        const int HEIGHT = 320;
        const int WIDTH = 320;
        const int X_OFFSET = 5;
        private int cx = X_OFFSET;
        // 实际的Y轴的位置
        int centerY = HEIGHT / 2;
        Timer timer = new Timer();
        TimerTask task;


        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
            Log.Debug(TAG, "当前线程ID" + Java.Lang.Thread.CurrentThread().Id);
            // Get our button from the layout resource,
            // and attach an event to it
            Button btn_sin = FindViewById<Button>(Resource.Id.btn_sin);
            Button btn_cos = FindViewById<Button>(Resource.Id.btn_cos);
            btn_sin.SetOnClickListener(this);
            btn_cos.SetOnClickListener(this);


            SurfaceView surface = (SurfaceView)FindViewById(Resource.Id.surface_view_wave);
            // 初始化SurfaceHolder对象
            holder = surface.Holder;
            paint = new Paint();
            paint.Color = Color.Green;
            paint.StrokeWidth = 3;

        }

        /// <summary>
        /// 实现了View组件的点击事件监听器的接口的回调方法
        /// </summary>
        /// <param name="v"></param>
        public void OnClick(View v)
        {
            drawBack(holder);
            cx = X_OFFSET;
            if (task != null)
            {
                task.Cancel();
            }
            task = new MyTimerTask(v, this);
            timer.Schedule(task, 0, 30);
            holder.AddCallback(this);
        }

        /// <summary>
        /// 在每次点击按钮的时候绘制坐标轴
        /// </summary>
        /// <param name="holder"></param>
        private void drawBack(ISurfaceHolder holder)
        {
            Canvas canvas = holder.LockCanvas();
            // 绘制白色背景
            canvas.DrawColor(Color.White);
            Paint p = new Paint();
            p.Color = Color.Black;
            p.StrokeWidth = 2;
            // 绘制坐标轴
            canvas.DrawLine(X_OFFSET, centerY, WIDTH, centerY, p);
            canvas.DrawLine(X_OFFSET, 40, X_OFFSET, HEIGHT, p);
            holder.UnlockCanvasAndPost(canvas);
            holder.LockCanvas(new Rect(0, 0, 0, 0));
            holder.UnlockCanvasAndPost(canvas);
        }

        /// <summary>
        /// 实现自定义的定时任务类,继承TimerTask类,并重写了Run()回调方法
        /// </summary>
        class MyTimerTask : TimerTask
        {

            bool isFirst = true;

            /// <summary>
            /// 点击事件的回调方法的参数 v
            /// </summary>
            View v;

            /// <summary>
            /// 主活动的引用
            /// </summary>
            MainActivity ma;

            /// <summary>
            /// 为自定义的定时任务类特定创建的构造方法
            /// 内部类不能够直接访问主活动的属性和方法
            /// 所以必须先获取控件的引用和主活动的引用
            /// 或者让MainActivity继承TimerTask也可以
            /// </summary>
            /// <param name="v">点击事件的参数 v,代表一个控件对象</param>
            /// <param name="ma">主活动的引用</param>
            public MyTimerTask(View v, MainActivity ma) : base()
            {
                this.v = v;
                this.ma = ma;
            }

            /// <summary>
            /// 开启一个线程,执行耗时的操作
            /// </summary>
            public override void Run()
            {
                if (isFirst)
                {
                    Log.Debug(ma.TAG, "当前线程ID" + Java.Lang.Thread.CurrentThread().Id);
                    isFirst = false;
                }
                int cy = v.Id == Resource.Id.btn_sin
                    ? ma.centerY - (int)(100 * Math.Sin((ma.cx - 5) * 2 * Math.PI / 150))
                    : ma.centerY - (int)(100 * Math.Cos((ma.cx - 5) * 2 * Math.PI / 150));
                Canvas canvas = ma.holder.LockCanvas(new Rect(ma.cx, cy - 2, ma.cx + 2, cy + 2));
                canvas.DrawPoint(ma.cx, cy, ma.paint);
                ma.cx++;
                if (ma.cx > WIDTH)
                {
                    Cancel();
                    ma.task = null;
                }
                ma.holder.UnlockCanvasAndPost(canvas);
            }

        }


        public void SurfaceChanged(ISurfaceHolder holder, [GeneratedEnum] Format format, int width, int height)
        {
            Log.Debug(TAG, "SurfaceChanged方法正在执行");
            drawBack(holder);
        }

        public void SurfaceCreated(ISurfaceHolder holder)
        {
            Log.Debug(TAG, "SurfaceCreated方法正在执行");
        }

        public void SurfaceDestroyed(ISurfaceHolder holder)
        {
            Log.Debug(TAG, "SurfaceDestroyed方法正在执行");
            timer.Cancel();
        }


    }
}