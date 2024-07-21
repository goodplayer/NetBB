# NetBB

## 1. 介绍

## 2. ？？？

## 3. 项目编译

### 3.1 编译前端项目

进入到工程`NetBB.Frontend`

```text
npm run build
或
_build.bat
```

### 3.2 编译后端项目

## 4. 开发环境配置

### 4.1 前端开发环境配置

安装：

1. Node

初始化Preact前端项目

```text
npm init preact

常用命令：
1. 开发调试： npm run dev
2. 编译构建： npm run build
```

删除不需要的文件

```text
图片、css
js中import的图片css等

在aspnet core工程wwwroot下增加.gitignore文件，并将index.html排除，防止前端生成的测试用html进入代码库
```

安装代码格式化工具

```text
npm install --save-dev --save-exact prettier
node --eval "fs.writeFileSync('.prettierrc','{}\n')"
```

增加代码格式化命令，修改`package.json`文件scripts下面增加

```text
"format": "prettier . --write",
```

配置前端构建输出文件路径到aspnet core的wwwroot下面，同时名字中去掉hash

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

增加动态监控文件变动并编译的命令，修改`package.json`文件scripts下面增加

```text
"watchbuild": "vite build --watch"
```

### 4.1 后端开发环境配置

安装准备：

1. Dotnet SDK

