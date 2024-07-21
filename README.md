# NetBB

## 1. ����

## 2. ������

## 3. ��Ŀ����

### 3.1 ����ǰ����Ŀ

���뵽����`NetBB.Frontend`

```text
npm run build
��
_build.bat
```

### 3.2 ��������Ŀ

## 4. ������������

### 4.1 ǰ�˿�����������

��װ��

1. Node

��ʼ��Preactǰ����Ŀ

```text
npm init preact

�������
1. �������ԣ� npm run dev
2. ���빹���� npm run build
```

ɾ������Ҫ���ļ�

```text
ͼƬ��css
js��import��ͼƬcss��

��aspnet core����wwwroot������.gitignore�ļ�������index.html�ų�����ֹǰ�����ɵĲ�����html��������
```

��װ�����ʽ������

```text
npm install --save-dev --save-exact prettier
node --eval "fs.writeFileSync('.prettierrc','{}\n')"
```

���Ӵ����ʽ������޸�`package.json`�ļ�scripts��������

```text
"format": "prettier . --write",
```

����ǰ�˹�������ļ�·����aspnet core��wwwroot���棬ͬʱ������ȥ��hash

```text
  build: {
    outDir: "../NetBB/wwwroot",
    rollupOptions: {
      output: {
        entryFileNames: "assets/[name].js",
        assetFileNames: `assets/[name].[ext]`,
      },
    },
  },
```

���Ӷ�̬����ļ��䶯�����������޸�`package.json`�ļ�scripts��������

```text
"watchbuild": "vite build --watch"
```

### 4.1 ��˿�����������

��װ׼����

1. Dotnet SDK

