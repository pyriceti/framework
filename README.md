<h1 align="center">Pyriceti framework</h1>
<p align="center">
  <a href="https://openupm.com/packages/com.pyriceti.framework/">
    <img src="https://img.shields.io/npm/v/com.pyriceti.framework?label=openupm&registry_uri=https://package.openupm.com"  alt="OpenUPM Shield"/>
  </a>
</p>

All code, tools and resources from the Pyriceti framework to start a Unity project on fire!

Original author is [Baptiste Perraud aka Pyriceti](https://baptiste-perraud.com/).

##### Table of Contents

- [Install Package](#install-package)
    - [Install via OpenUPM](#install-via-openupm)
    - [Install via Git URL](#install-via-git-url)
- [What is this?](#what-is-this)
- [How to Use](#how-to-use)

## Install Package

### Install via OpenUPM

The package is available on the [openupm registry](https://openupm.com). It's recommended to install it
via [openupm-cli](https://github.com/openupm/openupm-cli).

```sh
npm install -g openupm-cli
openupm add com.pyriceti.framework
```

### Install via Git URL

Open `Packages/manifest.json` with your favorite text editor, then add a package entry in the dependencies block:

```json
{
  "dependencies": {
    "com.pyriceti.framework": "https://github.com/pyriceti/framework.git"
  }
}
```

Notice: Unity Package Manager records the current commit to a lock entry of `manifest.json` file. To update to the
latest version, change the hash value manually or remove the lock entry to resolve the package.

```json
{
  "lock": {
    "com.pyriceti.framework": {
      "revision": "main",
      "hash": "..."
    }
  }
}
```

## What is this?

Pyriceti framework is a framework I started to develop in order to capitalize the experience I got working on previous
projects. It is a homemade framework granting some utilities, tools and architecture inspired by my greatest learnings
and mistakes.

Feel free to use it for your own projects. I will try to update it and its documentation as often as possible.

## How to Use

See [usage](./PyricetiFramework/Documentation~/PyricetiFramework.md)
