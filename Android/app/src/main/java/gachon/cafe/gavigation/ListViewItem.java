package gachon.cafe.gavigation;

import android.graphics.drawable.Drawable;

public class ListViewItem {
    private Drawable iconDrawable;
    private String titleStr;
    private String senderStr;
    private String dateStr;

    public void setIcon(Drawable icon)
    {
        iconDrawable = icon;
    }
    public void setTitle(String title)
    {
        titleStr = title;
    }
    public void setSender(String sender) {senderStr = sender;}
    public void setDate(String date) {dateStr = date;}
    public Drawable getIcon()
    {
        return this.iconDrawable;
    }
    public String getTitle()
    {
        return this.titleStr;
    }
    public String getSenderStr()
    {
        return this.senderStr;
    }
    public String getDateStr()
    {
        return this.dateStr;
    }
}
