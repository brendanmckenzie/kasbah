import React from 'react'
import PropTypes from 'prop-types'
import moment from 'moment'
import { connect } from 'react-redux'
import { makeApiRequest } from 'store/util'

// TODO: this doesn't really belong under `components`

export class AuthManager extends React.Component {
  static propTypes = {
    children: PropTypes.any,
    auth: PropTypes.object.isRequired
  }

  static contextTypes = {
    router: PropTypes.object.isRequired
  }

  constructor() {
    super()

    this.state = {
      timeout: null
    }
  }

  componentDidMount() {
    this.checkRefresh()
  }

  componentWillDismount() {
    if (this.state.timeout) {
      clearTimeout(this.state.timeout)
    }
  }

  componentWillReceiveProps(nextProps) {
    if (nextProps.auth.authError && nextProps.auth.authError !== this.props.auth.authError) {
      this.context.router.push('/login')
    }
  }

  checkRefresh = () => {
    // TODO: move this to a reducer
    if (!localStorage.user || !localStorage.accessTokenExpires) {
      this.context.router.push('/login')
      return
    }

    if (moment().add(5, 'seconds').isAfter(moment(localStorage.accessTokenExpires))) {
      const user = JSON.parse(localStorage.user)
      console.log('issue refresh token request')
      makeApiRequest({
        url: '/connect/token',
        headers: {
          'Content-Type': 'application/x-www-form-urlencoded'
        },
        rawBody: `grant_type=refresh_token&refresh_token=${user.refresh_token}`
      })
        .then(res => {
          localStorage.user = JSON.stringify(res)
          localStorage.accessTokenExpires = moment().add(res.expires_in, 'seconds').format()

          this.queueCheckRefresh()
        })
        .catch(err => {
          console.error(err)

          this.context.router.push('/login')
        })
    } else {
      this.queueCheckRefresh()
    }
  }

  queueCheckRefresh() {
    try {
      this.setState({
        timeout: setTimeout(this.checkRefresh, 1000)
      })
    } catch (ex) {
      console.log('probably dismounted')
    }
  }

  render() {
    return this.props.children
  }
}

const mapDispatchToProps = {
}

const mapStateToProps = (state) => ({
  auth: state.auth
})

export default connect(mapStateToProps, mapDispatchToProps)(AuthManager)
