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
      <div className='hero is-fullheight'>
        <div className='hero-body'>
          <div className='container'>
            <div className='columns'>
              <div className='column is-4 is-offset-4'>
                <div className='notification is-info'>
                  <p className='heading'>Content authoring and marketing platform</p>
                  <div className='level'>
                    <div className='level-left'>
                      <p className='title'>Kasbah</p>
                    </div>
                    <div className='level-right'>
                      <small>v1.0.0-build-5602</small>
                    </div>
                  </div>
                </div>
                <LoginForm onSubmit={this.props.loginRequest} {...this.props.login} />
              </div>
            </div>
          </div>
        </div>
      </div>
    )
  }
}

export default View
