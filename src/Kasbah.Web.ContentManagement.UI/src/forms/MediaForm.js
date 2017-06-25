import React from 'react'
import PropTypes from 'prop-types'
import { Field, reduxForm } from 'redux-form'
import ModalForm from 'components/ModalForm'

const MediaForm = ({ handleSubmit, onClose, loading }) => (
  <ModalForm onClose={onClose} onSubmit={handleSubmit} loading={loading} title='Media details'>
    <div className='field'>
      <label className='label' htmlFor='fileName'>File name</label>
      <div className='control'>
        <Field name='fileName' component='input' type='text' className='input' autoFocus />
      </div>
    </div>
    <div className='field'>
      <label className='label' htmlFor='type'>Content type</label>
      <div className='control'>
        <Field name='contentType' component='input' type='text' className='input' />
      </div>
    </div>
  </ModalForm>
)

MediaForm.propTypes = {
  handleSubmit: PropTypes.func.isRequired,
  loading: PropTypes.bool,
  onClose: PropTypes.func.isRequired
}

export default reduxForm({
  form: 'MediaForm'
})(MediaForm)
