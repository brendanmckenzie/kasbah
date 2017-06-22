import React from 'react'
import PropTypes from 'prop-types'
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
  children: PropTypes.element.isRequired
}

export default CoreLayout
