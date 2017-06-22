import React from 'react'
import PropTypes from 'prop-types'

const ModalForm = ({ onSubmit, onClose, children, title, loading }) => (
  <div className='modal is-active'>
    <div className='modal-background' onClick={onClose} />
    <form onSubmit={onSubmit}>
      <div className='modal-card'>
        <header className='modal-card-head'>
          <span className='modal-card-title'>
            {title}
          </span>
          <button type='button' className='delete' onClick={onClose} />
        </header>
        <section className='modal-card-body'>
          {children}
        </section>
        <footer className='modal-card-foot'>
          <button type='button' className='button' onClick={onClose}>Cancel</button>
          <button type='submit' className={'button is-primary ' + (loading ? 'is-loading' : '')}>Save</button>
        </footer>
      </div>
    </form>
  </div>
)

ModalForm.propTypes = {
  onSubmit: PropTypes.func.isRequired,
  onClose: PropTypes.func.isRequired,
  loading: PropTypes.bool,
  children: PropTypes.any.isRequired,
  title: PropTypes.string.isRequired
}

export default ModalForm
