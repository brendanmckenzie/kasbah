import React from 'react'
import { Field, reduxForm } from 'redux-form'
import _ from 'lodash'
import { Tabs, Tab } from 'components/Tabs'
import kasbah from 'kasbah'

const ContentEditorForm = ({ handleSubmit, onPublish, type, loading, publishing }) => (
  <form onSubmit={handleSubmit} className='content-editor__form' disabled={loading || publishing}>
    <Tabs>
      {_(type.fields).map(ent => ent.category).uniq().value().map((ent, index) => (
        <Tab key={index} title={ent}>
          <div>
            {type.fields.filter(fld => fld.category === ent).map((fld, fldIndex) => (
              <div key={fldIndex} className='control'>
                <label className='label' htmlFor={fld.alias}>{fld.displayName}</label>
                <Field
                  name={fld.alias}
                  id={fld.alias}
                  component={kasbah.getEditor(fld.editor)}
                  options={fld.options}
                  className='input' />
                {fld.helpText && <span className='help'>{fld.helpText}</span>}
              </div>
            ))}
          </div>
        </Tab>
      ))}
    </Tabs>
    <hr />
    <div className='has-text-right'>
      <button
        className={'button ' + ((publishing || loading) ? 'is-loading' : '')}
        type='button' onClick={onPublish}>Publish</button>
      <button className={'button is-primary' + ((loading || loading) ? ' is-loading' : '')}>Save changes</button>
    </div>
  </form>
)

ContentEditorForm.propTypes = {
  handleSubmit: React.PropTypes.func.isRequired,
  onPublish: React.PropTypes.func.isRequired,
  type: React.PropTypes.object.isRequired,
  loading: React.PropTypes.bool,
  publishing: React.PropTypes.bool,
  error: React.PropTypes.string
}

export default reduxForm({
  form: 'ContentEditorForm'
})(ContentEditorForm)
