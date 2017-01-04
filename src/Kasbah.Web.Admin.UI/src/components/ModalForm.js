import React from 'react'

const ModalForm = ({ onSubmit, onClose, children, title }) => (
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
          <button type='submit' className='button is-primary'>Save</button>
        </footer>
      </div>
    </form>
  </div>
)

ModalForm.propTypes = {
  onSubmit: React.PropTypes.func.isRequired,
  onClose: React.PropTypes.func.isRequired,
  children: React.PropTypes.element.isRequired,
  title: React.PropTypes.string.isRequired
}

export default ModalForm
