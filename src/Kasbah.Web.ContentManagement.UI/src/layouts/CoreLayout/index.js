import React from 'react'
import Header from 'components/Header'
import 'styles/core.scss'

export const CoreLayout = ({ children }) => (
  <div>
    <Header />
    {children}
  </div>
)

CoreLayout.propTypes = {
  children : React.PropTypes.element.isRequired
}

export default CoreLayout