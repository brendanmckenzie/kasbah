import React from 'react'
import { Field, reduxForm } from 'redux-form'
import _ from 'lodash'
import { Tabs, Tab } from 'components/Tabs'
import { getEditor } from 'editors'

const ContentEditorForm = ({ handleSubmit, onPublish, type, loading }) => (
  <form onSubmit={handleSubmit}>
    <Tabs>
      {_(type.fields).map(ent => ent.category).uniq().value().map((ent, index) => (
        <Tab key={index} title={ent}>
          <div>
            {type.fields.filter(fld => fld.category === ent).map((fld, fldIndex) => (
              <div key={fldIndex} className='control'>
                <label className='label' htmlFor={fld.alias}>{fld.displayName}</label>
                <Field name={fld.alias} id={fld.alias} component={getEditor(fld.editor)} className='input' />
                {fld.helpText && <span className='help'>{fld.helpText}</span>}
              </div>
            ))}
          </div>
        </Tab>
      ))}
    </Tabs>
    <hr />
    <div className='control has-text-right'>
      <button className={'button is-primary' + (loading ? ' is-loading' : '')}>Save changes</button>
      <button className='button' type='button' onClick={onPublish}>Publish</button>
    </div>
  </form>
)

ContentEditorForm.propTypes = {
  handleSubmit: React.PropTypes.func.isRequired,
  onPublish: React.PropTypes.func.isRequired,
  type: React.PropTypes.object.isRequired,
  loading: React.PropTypes.bool,
  error: React.PropTypes.string
}

export default reduxForm({
  form: 'ContentEditorForm'
})(ContentEditorForm)
