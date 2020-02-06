# Interactive help
An example implementation of the interactive help [DotNetCore]

![img](https://github.com/ilya-negrub/HelpView/blob/master/HelpView/preview.gif)

Example script this view:
```XAML
 <controls:HelpViewPanel x:Name="main"
                         IsShow="True"
                         DurationViewHelpObject="0:0:2"
                         Background="#C8000000">
    <controls:HelpViewPanel.Script>                
        <!--ViewThree-->
        <controls:ScriptObject NameHelpObject="lbItems" 
                               IsShowItem="True" 
                               Description="Description of ListBox"
                               DescriptionTemplate="{StaticResource descCCTemplate}" 
                               DescriptionItem="Description of ListBoxItem"
                               DescriptionItemTemplate="{StaticResource descCLTemplate}">
            <controls:ScriptObject.Items>
                <controls:ScriptObject NameHelpObject="imgItem" 
                                       DescriptionTemplate="{StaticResource descCLTemplate}" 
                                       Description="Description of Image Item"/>
                <controls:ScriptObject NameHelpObject="contentItem" 
                                       DescriptionTemplate="{StaticResource descCLTemplate}" 
                                       Description="Description of Content Item"/>
            </controls:ScriptObject.Items>
        </controls:ScriptObject>                                       
        <controls:ScriptObject NameHelpObject="gridContent1" 
                               DescriptionTemplate="{StaticResource descCLTemplate}" 
                               Description="Desctiption of Content"/>
        <controls:ScriptObject NameHelpObject="lbContent" 
                               DescriptionTemplate="{StaticResource descTemplate}" 
                               Description="Desctiption of Label Content"/>
    </controls:HelpViewPanel.Script>
</controls:HelpViewPanel
```

Register ListBox:
```XAML
<ListBox x:Name="lbItems" 
         controls:HelpViewPanel.Attached="main"
         ItemsSource="{Binding ItemsSource}"/>
```
