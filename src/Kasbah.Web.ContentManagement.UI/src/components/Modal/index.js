import React from 'react'
import PropTypes from 'prop-types'

class Modal extends React.PureComponent {
  static propTypes = {
    modal: PropTypes.object
  }

  render() {
    const { modal } = this.props

    if (!modal) {
      return null
    }

    return <pre>{JSON.stringify(modal, null, 2)}</pre>
  }
}

export default Modal
