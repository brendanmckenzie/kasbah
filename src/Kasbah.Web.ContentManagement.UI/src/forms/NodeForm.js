import React from 'react'
import PropTypes from 'prop-types'
import { Field, reduxForm } from 'redux-form'
import ModalForm from 'components/ModalForm'

const NodeForm = ({ handleSubmit, types, onClose, loading }) => (
  <ModalForm onClose={onClose} onSubmit={handleSubmit} loading={loading} title='Create node'>
    <div className='field'>
      <label className='label' htmlFor='Type'>Type</label>
      <div className='control'>
        <span className='select is-fullwidth'>
          <Field name='type' component='select' autoFocus>
            {types
              .map((ent, index) => <option key={index} value={ent.alias}>{ent.displayName}</option>)}
          </Field>
        </span>
      </div>
    </div>
    <div className='field'>
      <label className='label' htmlFor='alias'>Alias</label>
      <div className='control'>
        <Field name='alias' component='input' type='text' className='input' />
      </div>
    </div>
    <div className='field'>
      <label className='label' htmlFor='displayName'>Display name</label>
      <div className='control'>
        <Field name='displayName' component='input' type='text' className='input' />
      </div>
    </div>
  </ModalForm>
)

NodeForm.propTypes = {
  handleSubmit: PropTypes.func.isRequired,
  loading: PropTypes.bool,
  onClose: PropTypes.func.isRequired,
  types: PropTypes.array.isRequired
}

export default reduxForm({
  form: 'NodeForm'
})(NodeForm)
