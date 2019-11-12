import React from 'react';
import ShallowRenderer from 'react-test-renderer/shallow';
import TemplatePreview from '../index';

describe('TemplatePreview', () => {
    it('render correctly', () => {
        let props = {
            text: '**bold**'
        };

        let result = (new ShallowRenderer()).render(<TemplatePreview {...props} />);

        expect(result).toMatchSnapshot();
    });
});