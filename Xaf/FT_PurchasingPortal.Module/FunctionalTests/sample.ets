;#DropDB FT_PurchasingPortalEasyTest

#Application FT_PurchasingPortalWin
#Application FT_PurchasingPortalWeb

*FillForm
 User Name = Admin
*Action Log In

#IfDef FT_PurchasingPortalWin

*Action About...
 
*Action OK

#EndIf

#IfDef FT_PurchasingPortalWeb
*Action Navigation(My Details)
#EndIf
