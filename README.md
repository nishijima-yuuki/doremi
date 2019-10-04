doremi
====
Microsoft Windowsのデスクトップアイコンを指定した座標へ移動します。

## システム要件

- OSがMicrosoft Windowsであること
- .NET Framework 4.0以上がインストールされていること

## 使いかた

    doremi.exe <アイコン名> <X座標>　<Y座標>

### "PC"アイコンをデスクトップの右上へ移動する例

```Batchfile
doremi.exe PC 9999 0
```

## 終了コード

- 0: 正常終了。
- 1: 引数異常。
- 2: アイコンが見つからない。
- 10: 実行時エラー。

## 故障かな?と思ったら

- 「アイコンの自動整列」が有効ではありませんか。無効にしてください。
- アイコン名としてファイル名を指定していませんか。ショートカットアイコンを移動する場合、".lnk"は不要です。デスクトップに表示されている名前を指定してください。

## 応用

### アイコンが生成されるまでリトライする

バッチやVBScript等からdoremi.exeを実行、終了コードが2である間、実行を続ければリトライできます。以下は"HogeIcon"が生成されるまでリトライするバッチの例です。

```Batchfile
:LOOP
doremi.exe HogeIcon 9999 0
IF %ERRORLEVEL%==2 (
    timeout 1
    GOTO LOOP
)
```

## ライセンス
Apache License 2.0です。
