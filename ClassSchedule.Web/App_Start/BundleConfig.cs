using System.Web;
using System.Web.Optimization;

namespace ClassSchedule.Web
{
    public class BundleConfig
    {
        //Дополнительные сведения об объединении см. по адресу: http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Используйте версию Modernizr для разработчиков, чтобы учиться работать. Когда вы будете готовы перейти к работе,
            // используйте средство сборки на сайте http://modernizr.com, чтобы выбрать только нужные тесты.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new ScriptBundle("~/bundles/upload").Include(
                      "~/Scripts/jquery.ui.widget.js",
                      /*"~/Scripts/jquery.iframe-transport.js",*/
                      "~/Scripts/jquery.fileupload.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/font-awesome.css",
                      "~/Content/site.css"));

            bundles.Add(new ScriptBundle("~/bundles/typeahead").Include(
                       "~/Scripts/bootstrap3-typeahead.js"));

            bundles.Add(new ScriptBundle("~/bundles/underscore").Include(
                       "~/Scripts/underscore.js"));

            bundles.Add(new ScriptBundle("~/bundles/knockout").Include(
                      "~/Scripts/knockout-3.4.0.js",
                      "~/Scripts/knockout.mapping-latest.js",
                      "~/Scripts/knockout.validation.js"));

            bundles.Add(new ScriptBundle("~/bundles/edit-lesson").Include(
                       "~/Scripts/lesson.edit.js"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap-select").Include(
                       "~/Scripts/bootstrap-select.js"));
            bundles.Add(new StyleBundle("~/Content/bootstrap-select").Include(
                      "~/Content/bootstrap-select.css"));

            bundles.Add(new StyleBundle("~/Content/login").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/font-awesome.css",
                      "~/Content/login.css"));

        }
    }
}
