<template>
  <div id="vueJSApp">
      <div class="container-fluid">
          <nav class="navbar fixed-top navbar-expand-lg navbar-light bg-light justify-content-between">
              <button class="navbar-toggler" type="button" data-toggle="collapse" data-target="#navbarSupportedContent" aria-controls="navbarSupportedContent" aria-expanded="false" aria-label="Toggle navigation">
                  <span class="navbar-toggler-icon"></span>
              </button>
              <div class="collapse navbar-collapse" id="navbarSupportedContent">
                  <ul class="navbar-nav mr-auto container-fluid justify-content-center">
                      <li class="nav-item" :class="{ active: isActive('home') }">
                          <router-link class="nav-link" href="#" to="/">Home</router-link>
                      </li>
                  </ul>
              </div>
          </nav>
      </div>
      <div class="app-container container">
          <router-view></router-view>
      </div>
  </div>
</template>

<script>
export default {
  name: 'App',
  methods: {
    isActive (name) {
      return name === this.$route.name
    }
  }
}
</script>

<style>
#app {
  font-family: 'Avenir', Helvetica, Arial, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  text-align: center;
  color: #2c3e50;
  margin-top: 60px;
}
</style>
