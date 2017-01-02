import React from 'react'
import LoginForm from '../forms/LoginForm'

export class View extends React.Component {
  static propTypes = {
    loginRequest: React.PropTypes.func.isRequired,
    login: React.PropTypes.object.isRequired
  }

  static contextTypes = {
    router: React.PropTypes.object.isRequired
  }

  componentWillReceiveProps(nextProps) {
    if (nextProps.login.success && nextProps.login.success !== this.props.login.success) {
      localStorage.user = JSON.stringify(nextProps.login.payload)

      this.context.router.push('/')
    }
  }

  render() {
    return (
      <div className='section'>
        <div className='container'>
          <div className='columns'>
            <div className='column is-4 is-offset-4'>
              <h1 className='title'>Login</h1>
              <LoginForm onSubmit={this.props.loginRequest} {...this.props.login} />
            </div>
            <div className='column is-2'>
              <h5 className='heading'>System information</h5>
              <p>Kasbah v1.0.0-build-5602</p>
            </div>
          </div>
        </div>
      </div>
    )
  }
}

export default View
