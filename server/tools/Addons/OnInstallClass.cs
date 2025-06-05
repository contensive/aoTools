
using Contensive.BaseClasses;
using System;
using System.Text;
//
namespace Contensive.Addons.Tools {
    /// <summary>
    /// 
    /// </summary>
    public class OnInstallClass : Contensive.BaseClasses.AddonBaseClass {
        //
        //====================================================================================================
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cp"></param>
        /// <returns></returns>
        public override object Execute(CPBaseClass cp) {
            try {
                //
                cp.Layout.updateLayout(Constants.layoutInviteProfileGuid, Constants.layoutInviteProfileName, Constants.layoutInviteProfileCdnPathFilename); 
                //
                return "";
            } catch (Exception ex) {
                cp.Site.ErrorReport(ex);
                throw;
            }
        }
    }
}

