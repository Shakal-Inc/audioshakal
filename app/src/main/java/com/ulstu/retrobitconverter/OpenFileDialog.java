package com.ulstu.retrobitconverter;

import android.Manifest;
import android.app.AlertDialog;
import android.content.Context;
import android.content.pm.PackageManager;
import android.os.Environment;
import android.util.Log;

import androidx.core.app.ActivityCompat;
import androidx.core.content.ContextCompat;

import java.io.File;

public class OpenFileDialog extends AlertDialog.Builder {

    private String currentPath = Environment.getExternalStorageDirectory().getPath();


    private String[] getFiles(String directoryPath){
        File directory = new File(directoryPath);
        File[] files = directory.listFiles();
        String[] result = new String[files.length];
        for (int i = 0; i < files.length; i++) {
            result[i] = files[i].getName();
        }
        return result;
    }

    public OpenFileDialog(Context context) {
        super(context);
        try {
            setPositiveButton(android.R.string.ok, null)
                    .setNegativeButton(android.R.string.cancel, null)
                    .setItems(getFiles(currentPath), null);
        } catch (Exception e) {
            Log.d("myTag", e + "");
        }
    }

}
