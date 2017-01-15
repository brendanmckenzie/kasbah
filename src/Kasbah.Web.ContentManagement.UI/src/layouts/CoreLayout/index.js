import React from 'react'
import Header from 'components/Header'
import AuthManager from 'components/AuthManager'
import 'styles/core.scss'

export const CoreLayout = ({ children }) => (
  <div>
    <Header />
    <AuthManager>
      {children}
    </AuthManager>
  </div>
)

CoreLayout.propTypes = {
  children: React.PropTypes.element.isRequired
}

export default CoreLayout
