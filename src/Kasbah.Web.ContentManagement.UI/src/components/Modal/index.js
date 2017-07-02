import React from 'react'
import PropTypes from 'prop-types'
import { connect } from 'react-redux'
import { actions as uiActions } from 'store/appReducers/ui'

class Modal extends React.PureComponent {
  static propTypes = {
    ui: PropTypes.object.isRequired,
    hideModal: PropTypes.func.isRequired
  }

  render() {
    const { modal } = this.props.ui

    if (!modal) {
      return null
    }

    const { title, buttons, control } = modal

    if (!title && !buttons) {
      return control
    }

    return (
      <div className='modal is-active'>
        <div className='modal-background' />
        <div className='modal-card'>
          <header className='modal-card-head'>
            <p className='modal-card-title'>{title}</p>
            <button className='delete' onClick={this.props.hideModal} />
          </header>
          <section className='modal-card-body'>
            {control}
          </section>
          <footer className='modal-card-foot'>
            {buttons}
          </footer>
        </div>
      </div>
    )
  }
}

const mapStateToProps = (state) => ({
  ui: state.ui
})

const mapDispatchToProps = {
  ...uiActions
}

export default connect(mapStateToProps, mapDispatchToProps)(Modal)
